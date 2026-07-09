using Entreprenly.WebServices.Inventory.Interfaces.Acl;
using Entreprenly.WebServices.Sales.Application.CommandServices;
using Entreprenly.WebServices.Sales.Domain.Model;
using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Model.Commands;
using Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Sales.Domain.Repositories;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Sales.Application.Internal.CommandServices;

/// <summary>
///     Handles sale commands. Registers the point-of-sale transaction and, once it is persisted,
///     deducts the sold quantities from inventory through the Inventory ACL.
/// </summary>
public class SaleCommandService(
    ISaleRepository saleRepository,
    IInventoryContextFacade inventoryContextFacade,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer,
    ILogger<SaleCommandService> logger)
    : ISaleCommandService
{
    /// <summary>
    ///     Validates and registers a sale, then deducts its items from inventory stock. Stock
    ///     deduction runs only after the sale is successfully persisted and never fails the sale.
    /// </summary>
    public async Task<Result<Sale>> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.OwnerEmail))
            return Result<Sale>.Failure(SalesError.OwnerRequired, localizer[nameof(SalesError.OwnerRequired)]);

        if (command.SellerId <= 0)
            return Result<Sale>.Failure(SalesError.SellerRequired, localizer[nameof(SalesError.SellerRequired)]);

        if (command.Items is null || command.Items.Count == 0)
            return Result<Sale>.Failure(SalesError.SaleItemsRequired, localizer[nameof(SalesError.SaleItemsRequired)]);

        var sale = new Sale(command.OwnerEmail, command.SellerId, command.Items, command.PaymentMethod,
            command.PaymentReceipt, command.Status);

        await saleRepository.AddAsync(sale, cancellationToken);
        var result = await CompleteAsync(sale, cancellationToken);

        if (result.IsSuccess)
            await DeductStockAsync(command, cancellationToken);

        return result;
    }

    /// <summary>
    ///     Deducts the sold quantities from inventory through the Inventory ACL. A failure here is
    ///     logged but never rolls back the registered sale, so the point-of-sale transaction stays
    ///     recorded even if stock could not be adjusted.
    /// </summary>
    private async Task DeductStockAsync(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        try
        {
            var deductions = command.Items
                .Select(item => new StockDeductionItem(item.ProductName, QuantityToDeduct(item)))
                .ToList();
            await inventoryContextFacade.DecrementStockForItemsAsync(command.OwnerEmail, deductions, cancellationToken);
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "[sales] stock decrement failed for owner {OwnerEmail}", command.OwnerEmail);
        }
    }

    /// <summary>
    ///     Resolves the amount to deduct for a line: units for unit products, kilograms for weight
    ///     products.
    /// </summary>
    private static double QuantityToDeduct(SaleItem item)
    {
        if (item.Quantity is not null) return item.Quantity.Value;
        if (item.WeightKg is not null) return item.WeightKg.Value;
        return 0d;
    }

    private async Task<Result<Sale>> CompleteAsync(Sale sale, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Sale>.Success(sale);
        }
        catch (OperationCanceledException)
        {
            return Result<Sale>.Failure(SalesError.OperationCancelled, localizer[nameof(SalesError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Sale>.Failure(SalesError.DatabaseError, localizer[nameof(SalesError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Sale>.Failure(SalesError.InternalServerError,
                localizer[nameof(SalesError.InternalServerError)]);
        }
    }
}
