namespace Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Discriminates how an inventory item is measured and sold. <see cref="Unit" /> items are
///     tracked and priced per unit, while <see cref="Weight" /> items are tracked and priced by
///     weight (kilograms).
/// </summary>
public enum ProductType
{
    Unit,
    Weight
}

public static class ProductTypeExtensions
{
    /// <summary>
    ///     Returns the lowercase wire value exposed to clients (<c>"unit"</c> / <c>"weight"</c>).
    /// </summary>
    public static string ToValue(this ProductType productType)
    {
        return productType switch
        {
            ProductType.Unit => "unit",
            ProductType.Weight => "weight",
            _ => productType.ToString().ToLowerInvariant()
        };
    }
}
