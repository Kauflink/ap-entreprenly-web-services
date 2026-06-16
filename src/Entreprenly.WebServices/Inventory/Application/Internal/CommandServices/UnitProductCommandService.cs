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
///     Handles unit product commands. All operations are scoped to the owner account.
/// </summary>
public class UnitProductCommandService(
    IUnitProductRepository unitProductRepository,
    IUnitLotRepository unitLotRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IUnitProductCommandService
{
    public async Task<Result<UnitProduct>> Handle(CreateUnitProductCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.OwnerEmail))
            return Failure<UnitProduct>(InventoryError.OwnerRequired);
        if (string.IsNullOrWhiteSpace(command.Name))
            return Failure<UnitProduct>(InventoryError.ProductNameRequired);
        if (!string.IsNullOrWhiteSpace(command.CodeQr) &&
            await unitProductRepository.ExistsByCodeQrAndOwnerEmailAsync(command.CodeQr, command.OwnerEmail,
                cancellationToken))
            return Failure<UnitProduct>(InventoryError.ProductCodeAlreadyExists);

        var unitProduct = new UnitProduct(command.OwnerEmail, command.Name, command.Description, command.CodeQr,
            command.Price, command.WeightGrams, command.Brand);

        await unitProductRepository.AddAsync(unitProduct, cancellationToken);
        return await CompleteAsync(unitProduct, cancellationToken);
    }

    public async Task<Result<UnitProduct>> Handle(UpdateUnitProductCommand command,
        CancellationToken cancellationToken)
    {
        var unitProduct = await unitProductRepository.FindByIdAndOwnerEmailAsync(command.UnitProductId,
            command.OwnerEmail, cancellationToken);
        if (unitProduct is null)
            return Failure<UnitProduct>(InventoryError.UnitProductNotFound);

        unitProduct.UpdateInfo(command.Name, command.Description, command.CodeQr, command.Price, command.WeightGrams,
            command.Brand);
        unitProductRepository.Update(unitProduct);
        return await CompleteAsync(unitProduct, cancellationToken);
    }

    public async Task<Result<int>> Handle(DeleteUnitProductCommand command, CancellationToken cancellationToken)
    {
        var unitProduct = await unitProductRepository.FindByIdAndOwnerEmailAsync(command.UnitProductId,
            command.OwnerEmail, cancellationToken);
        if (unitProduct is null)
            return Failure<int>(InventoryError.UnitProductNotFound);

        await unitLotRepository.RemoveByProductIdAndOwnerEmailAsync(command.UnitProductId, command.OwnerEmail,
            cancellationToken);
        unitProductRepository.Remove(unitProduct);
        return await CompleteAsync(command.UnitProductId, cancellationToken);
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
