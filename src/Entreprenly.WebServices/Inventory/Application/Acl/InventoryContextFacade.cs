using Entreprenly.WebServices.Inventory.Domain.Repositories;
using Entreprenly.WebServices.Inventory.Interfaces.Acl;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Inventory.Application.Acl;

/// <summary>
///     Application-layer implementation of the Inventory ACL facade. Computes catalog stock by
///     summing lots and deducts confirmed-order quantities consuming lots oldest-first (FIFO by
///     entry date), exposing both to other bounded contexts without coupling them to the Inventory
///     internal model.
/// </summary>
public class InventoryContextFacade(
    IUnitProductRepository unitProductRepository,
    IWeightProductRepository weightProductRepository,
    IUnitLotRepository unitLotRepository,
    IWeightLotRepository weightLotRepository,
    IUnitOfWork unitOfWork,
    ILogger<InventoryContextFacade> logger)
    : IInventoryContextFacade
{
    public async Task<IEnumerable<CatalogItem>> FetchCatalogByOwnerAsync(string ownerEmail,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ownerEmail)) return [];

        var catalog = new List<CatalogItem>();

        var unitLots = (await unitLotRepository.FindAllByOwnerEmailAsync(ownerEmail, cancellationToken)).ToList();
        foreach (var product in await unitProductRepository.FindAllByOwnerEmailAsync(ownerEmail, cancellationToken))
        {
            double stock = unitLots.Where(lot => lot.ProductId == product.Id).Sum(lot => lot.Quantity);
            catalog.Add(new CatalogItem(product.Name, product.Price, false, stock));
        }

        var weightLots = (await weightLotRepository.FindAllByOwnerEmailAsync(ownerEmail, cancellationToken)).ToList();
        foreach (var product in await weightProductRepository.FindAllByOwnerEmailAsync(ownerEmail, cancellationToken))
        {
            var stock = weightLots.Where(lot => lot.ProductId == product.Id).Sum(lot => lot.QuantityKg);
            catalog.Add(new CatalogItem(product.Name, product.PricePerKg, true, stock));
        }

        return catalog;
    }

    public async Task DecrementStockForItemsAsync(string ownerEmail, IEnumerable<StockDeductionItem> items,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ownerEmail)) return;
        var deductions = items?.ToList() ?? [];
        if (deductions.Count == 0) return;

        var unitProducts = (await unitProductRepository.FindAllByOwnerEmailAsync(ownerEmail, cancellationToken))
            .ToList();
        var weightProducts = (await weightProductRepository.FindAllByOwnerEmailAsync(ownerEmail, cancellationToken))
            .ToList();

        var changed = false;
        foreach (var item in deductions)
        {
            if (item?.ProductName is null || item.Quantity <= 0) continue;
            var name = item.ProductName.Trim();

            var unitProduct = unitProducts.FirstOrDefault(p =>
                !string.IsNullOrEmpty(p.Name) && string.Equals(p.Name.Trim(), name, StringComparison.OrdinalIgnoreCase));
            if (unitProduct is not null)
            {
                changed |= await DeductFromUnitLotsAsync(ownerEmail, unitProduct.Id, name, item.Quantity,
                    cancellationToken);
                continue;
            }

            var weightProduct = weightProducts.FirstOrDefault(p =>
                !string.IsNullOrEmpty(p.Name) && string.Equals(p.Name.Trim(), name, StringComparison.OrdinalIgnoreCase));
            if (weightProduct is not null)
            {
                changed |= await DeductFromWeightLotsAsync(ownerEmail, weightProduct.Id, name, item.Quantity,
                    cancellationToken);
                continue;
            }

            logger.LogWarning(
                "[inventory] confirmed order item '{ProductName}' (x{Quantity}) not found in {OwnerEmail}'s catalog; skipping",
                name, item.Quantity, ownerEmail);
        }

        if (changed) await unitOfWork.CompleteAsync(cancellationToken);
    }

    private async Task<bool> DeductFromUnitLotsAsync(string ownerEmail, int productId, string name, int quantity,
        CancellationToken cancellationToken)
    {
        var lots = (await unitLotRepository.FindAllByOwnerEmailAsync(ownerEmail, cancellationToken))
            .Where(lot => lot.ProductId == productId && lot.Quantity > 0)
            .OrderBy(lot => lot.EntryDate)
            .ToList();

        var changed = false;
        var remaining = quantity;
        foreach (var lot in lots)
        {
            if (remaining <= 0) break;
            remaining -= lot.Consume(remaining);
            unitLotRepository.Update(lot);
            changed = true;
        }

        if (remaining > 0)
            logger.LogWarning(
                "[inventory] not enough stock for unit product '{ProductName}' ({OwnerEmail}): {Remaining} unit(s) unfulfilled",
                name, ownerEmail, remaining);
        return changed;
    }

    private async Task<bool> DeductFromWeightLotsAsync(string ownerEmail, int productId, string name, double quantityKg,
        CancellationToken cancellationToken)
    {
        var lots = (await weightLotRepository.FindAllByOwnerEmailAsync(ownerEmail, cancellationToken))
            .Where(lot => lot.ProductId == productId && lot.QuantityKg > 0)
            .OrderBy(lot => lot.EntryDate)
            .ToList();

        var changed = false;
        var remaining = quantityKg;
        foreach (var lot in lots)
        {
            if (remaining <= 0) break;
            remaining -= lot.Consume(remaining);
            weightLotRepository.Update(lot);
            changed = true;
        }

        if (remaining > 0.0001)
            logger.LogWarning(
                "[inventory] not enough stock for weight product '{ProductName}' ({OwnerEmail}): {Remaining} kg unfulfilled",
                name, ownerEmail, remaining);
        return changed;
    }
}
