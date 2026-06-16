namespace Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;

/// <summary>
///     Proof of payment attached to a sale once its payment has been confirmed.
/// </summary>
/// <param name="Method">The payment method used.</param>
/// <param name="TransactionCode">The external transaction code (e.g. Yape/Plin operation number).</param>
/// <param name="Amount">The amount paid.</param>
/// <param name="ConfirmedAt">The instant the payment was confirmed.</param>
public record PaymentReceipt(
    PaymentMethod Method,
    string? TransactionCode,
    double Amount,
    DateTimeOffset ConfirmedAt);
