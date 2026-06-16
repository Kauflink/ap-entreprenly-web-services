using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

/// <summary>
///     A line item within a sale. On creation the <see cref="Subtotal" /> is ignored and recomputed
///     by the server.
/// </summary>
[SwaggerSchema("A line item within a sale")]
public record SaleItemResource(
    long ProductId,
    string ProductName,
    int? Quantity,
    double? WeightKg,
    double UnitPrice,
    double Subtotal);
