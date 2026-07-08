using Entreprenly.WebServices.Inventory.Application.CommandServices;
using Entreprenly.WebServices.Inventory.Domain.Model;
using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Domain.Repositories;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Inventory.Application.Internal.CommandServices;

/// <summary>
///     Handles weight lot commands. All operations are scoped to the owner account.
/// </summary>
public class WeightLotCommandService(
    IWeightLotRepository weightLotRepository,
    IWeightProductRepository weightProductRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IWeightLotCommandService
{
    public async Task<Result<WeightLot>> Handle(CreateWeightLotCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.OwnerEmail))
            return Failure<WeightLot>(InventoryError.OwnerRequired);
        if (!await weightProductRepository.ExistsByIdAndOwnerEmailAsync(command.ProductId, command.OwnerEmail,
                cancellationToken))
            return Failure<WeightLot>(InventoryError.WeightProductNotFound);
        if (command.QuantityKg < 0)
            return Failure<WeightLot>(InventoryError.NegativeQuantity);

        var weightLot = new WeightLot(command.OwnerEmail, command.ProductId, command.CodeQr, command.EntryDate,
            command.QuantityKg);

        await weightLotRepository.AddAsync(weightLot, cancellationToken);
        return await CompleteAsync(weightLot, cancellationToken);
    }

    public async Task<Result<WeightLot>> Handle(UpdateWeightLotCommand command, CancellationToken cancellationToken)
    {
        if (!await weightProductRepository.ExistsByIdAndOwnerEmailAsync(command.ProductId, command.OwnerEmail,
                cancellationToken))
            return Failure<WeightLot>(InventoryError.WeightProductNotFound);

        var weightLot = await weightLotRepository.FindByIdAndOwnerEmailAsync(command.WeightLotId, command.OwnerEmail,
            cancellationToken);
        if (weightLot is null)
            return Failure<WeightLot>(InventoryError.WeightLotNotFound);

        weightLot.UpdateInfo(command.ProductId, command.CodeQr, command.EntryDate, command.QuantityKg);
        weightLotRepository.Update(weightLot);
        return await CompleteAsync(weightLot, cancellationToken);
    }

    public async Task<Result<int>> Handle(DeleteWeightLotCommand command, CancellationToken cancellationToken)
    {
        var weightLot = await weightLotRepository.FindByIdAndOwnerEmailAsync(command.WeightLotId, command.OwnerEmail,
            cancellationToken);
        if (weightLot is null)
            return Failure<int>(InventoryError.WeightLotNotFound);

        weightLotRepository.Remove(weightLot);
        return await CompleteAsync(command.WeightLotId, cancellationToken);
    }

    private Result<T> Failure<T>(InventoryError error)
    {
        return Result<T>.Failure(error, localizer[error.ToString()]);
    }

    private async Task<Result<T>> CompleteAsync<T>(T value, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<T>.Success(value);
        }
        catch (OperationCanceledException)
        {
            return Failure<T>(InventoryError.OperationCancelled);
        }
        catch (DbUpdateException)
        {
            return Failure<T>(InventoryError.DatabaseError);
        }
        catch (Exception)
        {
            return Failure<T>(InventoryError.InternalServerError);
        }
    }
}
