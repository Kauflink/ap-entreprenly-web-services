using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;

/// <summary>
///     Unit lot aggregate root. Represents a batch of a <see cref="UnitProduct" /> that entered
///     stock on a given <see cref="EntryDate" />, owned by the account (<see cref="OwnerEmail" />)
///     that created it. Its <see cref="ProductType" /> is always <see cref="ProductType.Unit" />.
/// </summary>
public partial class UnitLot
{
    public UnitLot()
    {
        OwnerEmail = string.Empty;
    }

    public UnitLot(string ownerEmail, int productId, string? codeQr, DateTimeOffset? entryDate, int quantity,
        DateTimeOffset? expiryDate)
    {
        OwnerEmail = ownerEmail;
        ProductId = productId;
        CodeQr = codeQr;
        EntryDate = entryDate ?? DateTimeOffset.UtcNow;
        Quantity = quantity;
        ExpiryDate = expiryDate;
    }

    public int Id { get; }
    public string OwnerEmail { get; private set; }
    public int ProductId { get; private set; }
    public string? CodeQr { get; private set; }
    public DateTimeOffset EntryDate { get; private set; }
    public int Quantity { get; private set; }
    public DateTimeOffset? ExpiryDate { get; private set; }

    /// <summary>
    ///     The measurement type of this lot, always <see cref="ProductType.Unit" />.
    /// </summary>
    public ProductType ProductType => ProductType.Unit;

    /// <summary>
    ///     Updates the editable attributes of this lot.
    /// </summary>
    public UnitLot UpdateInfo(int productId, string? codeQr, DateTimeOffset? entryDate, int quantity,
        DateTimeOffset? expiryDate)
    {
        ProductId = productId;
        CodeQr = codeQr;
        if (entryDate.HasValue) EntryDate = entryDate.Value;
        Quantity = quantity;
        ExpiryDate = expiryDate;
        return this;
    }

    /// <summary>
    ///     Removes up to <paramref name="amount" /> units from this lot, never going below zero.
    /// </summary>
    /// <param name="amount">The number of units requested for removal.</param>
    /// <returns>The units actually removed (capped at the available quantity).</returns>
    public int Consume(int amount)
    {
        if (amount <= 0) return 0;
        var removed = Math.Min(amount, Quantity);
        Quantity -= removed;
        return removed;
    }
}
