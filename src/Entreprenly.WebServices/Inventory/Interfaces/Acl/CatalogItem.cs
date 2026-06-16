namespace Entreprenly.WebServices.Inventory.Interfaces.Acl;

/// <summary>
///     Published catalog snapshot of an inventory product, exposed to other bounded contexts.
/// </summary>
/// <param name="Name">The product display name.</param>
/// <param name="Price">The unit price for unit products, or the price per kilogram for weight products.</param>
/// <param name="ByWeight">Whether the product is sold by weight.</param>
/// <param name="Stock">The currently available stock (units or kilograms).</param>
public record CatalogItem(string Name, double Price, bool ByWeight, double Stock);
