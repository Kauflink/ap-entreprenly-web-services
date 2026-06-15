using Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Sales.Domain.Model.Commands;

/// <summary>
///     Command to register a new sale.
/// </summary>
/// <param name="OwnerEmail">The authenticated account that owns the sale.</param>
/// <param name="SellerId">The seller that performed the sale.</param>
/// <param name="Items">The sale line items.</param>
/// <param name="PaymentMethod">The payment method used.</param>
/// <param name="PaymentReceipt">The proof of payment (null until payment is confirmed).</param>
/// <param name="Status">The lifecycle status to register the sale with (null; defaults applied).</param>
public record CreateSaleCommand(
    string OwnerEmail,
    long SellerId,
    List<SaleItem> Items,
    PaymentMethod? PaymentMethod,
    PaymentReceipt? PaymentReceipt,
    SaleStatus? Status);
