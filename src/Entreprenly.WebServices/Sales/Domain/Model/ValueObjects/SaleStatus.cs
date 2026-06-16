namespace Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;

/// <summary>
///     Lifecycle status of a sale.
/// </summary>
public enum SaleStatus
{
    InProgress,
    PaymentConfirmed,
    Completed,
    Cancelled
}
