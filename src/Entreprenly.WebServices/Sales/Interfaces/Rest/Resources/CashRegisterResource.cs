using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

/// <summary>
///     Daily cash register summary returned by the REST API.
/// </summary>
[SwaggerSchema("Daily cash register summary")]
public record CashRegisterResource(
    int Id,
    DateOnly Date,
    double TotalCash,
    double TotalDigital,
    int SaleCount);
