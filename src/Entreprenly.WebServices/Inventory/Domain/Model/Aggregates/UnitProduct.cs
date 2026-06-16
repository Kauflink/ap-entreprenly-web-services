using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;

/// <summary>
///     Unit product aggregate root. Represents a catalog product that is tracked and sold per unit.
///     It belongs to the account (<see cref="OwnerEmail" />) that created it, so inventory is
///     isolated per tenant. Its <see cref="ProductType" /> is always <see cref="ProductType.Unit" />.
/// </summary>
public partial class UnitProduct
{
    public UnitProduct()
    {
        OwnerEmail = string.Empty;
        Name = string.Empty;
    }

    public UnitProduct(string ownerEmail, string name, string? description, string? codeQr, double price,
        double weightGrams, string? brand)
    {
        OwnerEmail = ownerEmail;
        Name = name;
        Description = description;
        CodeQr = codeQr;
        Price = price;
        WeightGrams = weightGrams;
        Brand = brand;
    }

    public int Id { get; }
    public string OwnerEmail { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public string? CodeQr { get; private set; }
    public double Price { get; private set; }
    public double WeightGrams { get; private set; }
    public string? Brand { get; private set; }

    /// <summary>
    ///     The measurement type of this product, always <see cref="ProductType.Unit" />.
    /// </summary>
    public ProductType ProductType => ProductType.Unit;

    /// <summary>
    ///     Updates the editable attributes of this product.
    /// </summary>
    public UnitProduct UpdateInfo(string name, string? description, string? codeQr, double price,
        double weightGrams, string? brand)
    {
        Name = name;
        Description = description;
        CodeQr = codeQr;
        Price = price;
        WeightGrams = weightGrams;
        Brand = brand;
        return this;
    }
}
