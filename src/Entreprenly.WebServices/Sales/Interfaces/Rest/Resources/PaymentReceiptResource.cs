using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

/// <summary>
///     Proof of payment attached to a sale.
/// </summary>
[SwaggerSchema("Proof of payment attached to a sale")]
public record PaymentReceiptResource(
    string Method,
    string? TransactionCode,
    double Amount,
    DateTimeOffset ConfirmedAt);
