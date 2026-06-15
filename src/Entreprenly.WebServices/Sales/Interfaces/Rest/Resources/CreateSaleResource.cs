using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

/// <summary>
///     Request to register a sale. The server is the source of truth for the total and line
///     subtotals, so those values are recomputed regardless of what the client sends.
/// </summary>
[SwaggerSchema("Request to register a sale")]
public record CreateSaleResource(
    [Required] long SellerId,
    [Required] [MinLength(1)] List<SaleItemResource> Items,
    string? PaymentMethod,
    PaymentReceiptResource? PaymentReceipt,
    string? Status);
