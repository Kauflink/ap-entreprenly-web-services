using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

/// <summary>
///     A registered sale returned by the REST API.
/// </summary>
[SwaggerSchema("A registered sale")]
public record SaleResource(
    int Id,
    long SellerId,
    List<SaleItemResource> Items,
    double Total,
    string PaymentMethod,
    PaymentReceiptResource? PaymentReceipt,
    string Status,
    DateTimeOffset CreatedAt,
    DateTimeOffset? CompletedAt);
