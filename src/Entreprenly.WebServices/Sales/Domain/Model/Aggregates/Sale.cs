using Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Sales.Domain.Model.Aggregates;

/// <summary>
///     Sale aggregate root.
/// </summary>
/// <remarks>
///     Represents a point-of-sale transaction made by a seller. It belongs to the account
///     (<see cref="OwnerEmail" />) that registered it, so sales are isolated per tenant. It holds the
///     list of <see cref="SaleItem" /> lines, the authoritative <see cref="Total" />, the chosen
///     <see cref="PaymentMethod" />, an optional <see cref="PaymentReceipt" /> and the lifecycle
///     <see cref="Status" />. The total is always recomputed from the line items so the server
///     remains the source of truth.
/// </remarks>
public partial class Sale
{
    public Sale()
    {
        OwnerEmail = string.Empty;
        Items = [];
        PaymentMethod = PaymentMethod.Cash;
        Status = SaleStatus.InProgress;
    }

    public Sale(string ownerEmail, long sellerId, List<SaleItem> items, PaymentMethod? paymentMethod,
        PaymentReceipt? paymentReceipt, SaleStatus? status)
    {
        OwnerEmail = ownerEmail;
        SellerId = sellerId;
        Items = items is null ? [] : [..items];
        Total = ComputeTotal(Items);
        PaymentMethod = paymentMethod ?? PaymentMethod.Cash;
        PaymentReceipt = paymentReceipt;
        Status = status ?? SaleStatus.InProgress;
        CreatedAt = DateTimeOffset.UtcNow;
        CompletedAt = Status == SaleStatus.Completed ? CreatedAt : null;
    }

    public int Id { get; }
    public string OwnerEmail { get; private set; }
    public long SellerId { get; private set; }
    public List<SaleItem> Items { get; private set; }
    public double Total { get; private set; }
    public PaymentMethod PaymentMethod { get; private set; }
    public PaymentReceipt? PaymentReceipt { get; private set; }
    public SaleStatus Status { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }

    /// <summary>
    ///     Recomputes the sale total as the sum of every line subtotal.
    /// </summary>
    private static double ComputeTotal(IEnumerable<SaleItem> items)
    {
        var raw = items.Sum(item => item.Subtotal);
        return Math.Round(raw * 100.0) / 100.0;
    }
}
