using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;

/// <summary>
///     Weight lot aggregate root. Represents a batch of a <see cref="WeightProduct" /> that entered
///     stock on a given <see cref="EntryDate" />, owned by the account (<see cref="OwnerEmail" />)
///     that created it. Its <see cref="ProductType" /> is always <see cref="ProductType.Weight" />.
/// </summary>
public partial class WeightLot
{
    public WeightLot()
    {
        OwnerEmail = string.Empty;
    }

    public WeightLot(string ownerEmail, int productId, string? codeQr, DateTimeOffset? entryDate, double quantityKg)
    {
        OwnerEmail = ownerEmail;
        ProductId = productId;
        CodeQr = codeQr;
        EntryDate = entryDate ?? DateTimeOffset.UtcNow;
        QuantityKg = quantityKg;
    }

    public int Id { get; }
    public string OwnerEmail { get; private set; }
    public int ProductId { get; private set; }
    public string? CodeQr { get; private set; }
    public DateTimeOffset EntryDate { get; private set; }
    public double QuantityKg { get; private set; }

    /// <summary>
    ///     The measurement type of this lot, always <see cref="ProductType.Weight" />.
    /// </summary>
    public ProductType ProductType => ProductType.Weight;

    /// <summary>
    ///     Updates the editable attributes of this lot.
    /// </summary>
    public WeightLot UpdateInfo(int productId, string? codeQr, DateTimeOffset? entryDate, double quantityKg)
    {
        ProductId = productId;
        CodeQr = codeQr;
        if (entryDate.HasValue) EntryDate = entryDate.Value;
        QuantityKg = quantityKg;
        return this;
    }

    /// <summary>
    ///     Removes up to <paramref name="amountKg" /> kilograms from this lot, never going below zero.
    /// </summary>
    /// <param name="amountKg">The kilograms requested for removal.</param>
    /// <returns>The kilograms actually removed (capped at the available quantity).</returns>
    public double Consume(double amountKg)
    {
        if (amountKg <= 0) return 0;
        var removed = Math.Min(amountKg, QuantityKg);
        QuantityKg = Math.Round((QuantityKg - removed) * 1000.0) / 1000.0;
        return removed;
    }
}
