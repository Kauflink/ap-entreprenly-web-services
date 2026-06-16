namespace Entreprenly.WebServices.Inventory.Interfaces.Acl;

/// <summary>
///     A quantity of a named product to deduct from inventory stock, used by other bounded contexts
///     through the <see cref="IInventoryContextFacade" />.
/// </summary>
/// <param name="ProductName">The product display name, matched case-insensitively.</param>
/// <param name="Quantity">The quantity to deduct (units for unit products, kilograms for weight products).</param>
public record StockDeductionItem(string ProductName, int Quantity);
