namespace Entreprenly.WebServices.Inventory.Interfaces.Acl;

/// <summary>
///     Anti-corruption layer that exposes Inventory bounded context capabilities to other contexts.
///     Provides a simplified integration surface for reading a seller's catalog with computed stock
///     and for deducting confirmed-order quantities, without leaking the Inventory internal model.
/// </summary>
public interface IInventoryContextFacade
{
    /// <summary>
    ///     Returns the catalog (products with price and computed stock) owned by a seller.
    /// </summary>
    Task<IEnumerable<CatalogItem>> FetchCatalogByOwnerAsync(string ownerEmail, CancellationToken cancellationToken);

    /// <summary>
    ///     Deducts the given quantities from the seller's stock, consuming lots oldest-first.
    /// </summary>
    Task DecrementStockForItemsAsync(string ownerEmail, IEnumerable<StockDeductionItem> items,
        CancellationToken cancellationToken);
}
