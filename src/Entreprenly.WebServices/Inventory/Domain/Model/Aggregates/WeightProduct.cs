using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;

/// <summary>
///     Weight product aggregate root. Represents a catalog product that is tracked and sold by
///     weight, owned by the account (<see cref="OwnerEmail" />) that created it. Its commercial
///     attribute is the <see cref="PricePerKg" />. Its <see cref="ProductType" /> is always
///     <see cref="ProductType.Weight" />.
/// </summary>
public partial class WeightProduct
{
    public WeightProduct()
    {
        OwnerEmail = string.Empty;
        Name = string.Empty;
    }

    public WeightProduct(string ownerEmail, string name, string? description, string? codeQr, double pricePerKg)
    {
        OwnerEmail = ownerEmail;
        Name = name;
        Description = description;
        CodeQr = codeQr;
        PricePerKg = pricePerKg;
    }

    public int Id { get; }
    public string OwnerEmail { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? CodeQr { get; private set; }
    public double PricePerKg { get; private set; }

    /// <summary>
    ///     The measurement type of this product, always <see cref="ProductType.Weight" />.
    /// </summary>
    public ProductType ProductType => ProductType.Weight;

    /// <summary>
    ///     Updates the editable attributes of this product.
    /// </summary>
    public WeightProduct UpdateInfo(string name, string? description, string? codeQr, double pricePerKg)
    {
        Name = name;
        Description = description;
        CodeQr = codeQr;
        PricePerKg = pricePerKg;
        return this;
    }
}
