using Entreprenly.WebServices.Inventory.Application.CommandServices;
using Entreprenly.WebServices.Inventory.Domain.Model;
using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Domain.Repositories;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Inventory.Application.Internal.CommandServices;

/// <summary>
///     Handles unit lot commands. All operations are scoped to the owner account.
/// </summary>
public class UnitLotCommandService(
    IUnitLotRepository unitLotRepository,
    IUnitProductRepository unitProductRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IUnitLotCommandService
{
    public async Task<Result<UnitLot>> Handle(CreateUnitLotCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.OwnerEmail))
            return Failure<UnitLot>(InventoryError.OwnerRequired);
        if (!await unitProductRepository.ExistsByIdAndOwnerEmailAsync(command.ProductId, command.OwnerEmail,
                cancellationToken))
            return Failure<UnitLot>(InventoryError.UnitProductNotFound);
        if (command.Quantity < 0)
            return Failure<UnitLot>(InventoryError.NegativeQuantity);

        var unitLot = new UnitLot(command.OwnerEmail, command.ProductId, command.CodeQr, command.EntryDate,
            command.Quantity, command.ExpiryDate);

        await unitLotRepository.AddAsync(unitLot, cancellationToken);
        return await CompleteAsync(unitLot, cancellationToken);
    }

    public async Task<Result<UnitLot>> Handle(UpdateUnitLotCommand command, CancellationToken cancellationToken)
    {
        if (!await unitProductRepository.ExistsByIdAndOwnerEmailAsync(command.ProductId, command.OwnerEmail,
                cancellationToken))
            return Failure<UnitLot>(InventoryError.UnitProductNotFound);

        var unitLot = await unitLotRepository.FindByIdAndOwnerEmailAsync(command.UnitLotId, command.OwnerEmail,
            cancellationToken);
        if (unitLot is null)
            return Failure<UnitLot>(InventoryError.UnitLotNotFound);

        unitLot.UpdateInfo(command.ProductId, command.CodeQr, command.EntryDate, command.Quantity, command.ExpiryDate);
        unitLotRepository.Update(unitLot);
        return await CompleteAsync(unitLot, cancellationToken);
    }

    public async Task<Result<int>> Handle(DeleteUnitLotCommand command, CancellationToken cancellationToken)
    {
        var unitLot = await unitLotRepository.FindByIdAndOwnerEmailAsync(command.UnitLotId, command.OwnerEmail,
            cancellationToken);
        if (unitLot is null)
            return Failure<int>(InventoryError.UnitLotNotFound);

        unitLotRepository.Remove(unitLot);
        return await CompleteAsync(command.UnitLotId, cancellationToken);
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
