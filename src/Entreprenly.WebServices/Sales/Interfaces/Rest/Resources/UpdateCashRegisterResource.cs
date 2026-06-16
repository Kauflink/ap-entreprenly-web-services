using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

/// <summary>
///     Request to update the running totals of a cash register.
/// </summary>
[SwaggerSchema("Request to update a cash register")]
public record UpdateCashRegisterResource(
    double TotalCash,
    double TotalDigital,
    int SaleCount);
