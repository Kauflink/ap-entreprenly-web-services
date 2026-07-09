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
///     Handles weight product commands. All operations are scoped to the owner account.
/// </summary>
public class WeightProductCommandService(
    IWeightProductRepository weightProductRepository,
    IWeightLotRepository weightLotRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IWeightProductCommandService
{
    public async Task<Result<WeightProduct>> Handle(CreateWeightProductCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.OwnerEmail))
            return Failure<WeightProduct>(InventoryError.OwnerRequired);
        if (string.IsNullOrWhiteSpace(command.Name))
            return Failure<WeightProduct>(InventoryError.ProductNameRequired);
        if (!string.IsNullOrWhiteSpace(command.CodeQr) &&
            await weightProductRepository.ExistsByCodeQrAndOwnerEmailAsync(command.CodeQr, command.OwnerEmail,
                cancellationToken))
            return Failure<WeightProduct>(InventoryError.ProductCodeAlreadyExists);

        var weightProduct = new WeightProduct(command.OwnerEmail, command.Name, command.Description, command.CodeQr,
            command.PricePerKg);

        await weightProductRepository.AddAsync(weightProduct, cancellationToken);
        return await CompleteAsync(weightProduct, cancellationToken);
    }

    public async Task<Result<WeightProduct>> Handle(UpdateWeightProductCommand command,
        CancellationToken cancellationToken)
    {
        var weightProduct = await weightProductRepository.FindByIdAndOwnerEmailAsync(command.WeightProductId,
            command.OwnerEmail, cancellationToken);
        if (weightProduct is null)
            return Failure<WeightProduct>(InventoryError.WeightProductNotFound);

        weightProduct.UpdateInfo(command.Name, command.Description, command.CodeQr, command.PricePerKg);
        weightProductRepository.Update(weightProduct);
        return await CompleteAsync(weightProduct, cancellationToken);
    }

    public async Task<Result<int>> Handle(DeleteWeightProductCommand command, CancellationToken cancellationToken)
    {
        var weightProduct = await weightProductRepository.FindByIdAndOwnerEmailAsync(command.WeightProductId,
            command.OwnerEmail, cancellationToken);
        if (weightProduct is null)
            return Failure<int>(InventoryError.WeightProductNotFound);

        await weightLotRepository.RemoveByProductIdAndOwnerEmailAsync(command.WeightProductId, command.OwnerEmail,
            cancellationToken);
        weightProductRepository.Remove(weightProduct);
        return await CompleteAsync(command.WeightProductId, cancellationToken);
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
