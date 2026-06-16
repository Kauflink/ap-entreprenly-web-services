using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

/// <summary>
///     Request to open a cash register for a business day.
/// </summary>
[SwaggerSchema("Request to open a cash register")]
public record CreateCashRegisterResource(
    [Required] DateOnly Date,
    double TotalCash,
    double TotalDigital);
