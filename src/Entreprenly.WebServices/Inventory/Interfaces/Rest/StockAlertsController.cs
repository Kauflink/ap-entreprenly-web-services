using System.Net.Mime;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest;

/// <summary>
///     Exposes derived stock alerts. Alerts are computed on demand from the current inventory
///     products and lots, so this controller only offers read operations.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/inventory-stock-alerts")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Derived inventory stock alert endpoints")]
public class StockAlertsController(
    IStockAlertQueryService stockAlertQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : InventoryControllerBase
{
    [HttpGet]
    [SwaggerOperation("List stock alerts",
        "Computes and retrieves every currently raised stock alert, ordered by descending urgency.",
        OperationId = "GetAllStockAlerts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Stock alerts computed", typeof(IEnumerable<StockAlertResource>))]
    public async Task<IActionResult> GetAllStockAlerts(CancellationToken cancellationToken)
    {
        var alerts = await stockAlertQueryService.Handle(new GetAllStockAlertsQuery(OwnerEmail), cancellationToken);
        return Ok(alerts.Select(StockAlertResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{stockAlertId:int}")]
    [SwaggerOperation("Get stock alert by id",
        "Retrieves a single currently raised stock alert by its run-scoped identifier.",
        OperationId = "GetStockAlertById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Stock alert found", typeof(StockAlertResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Stock alert not found")]
    public async Task<IActionResult> GetStockAlertById(int stockAlertId, CancellationToken cancellationToken)
    {
        var alert = await stockAlertQueryService.Handle(new GetStockAlertByIdQuery(OwnerEmail, stockAlertId),
            cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromEntity(this, alert, InventoryError.UnitLotNotFound,
            errorLocalizer, problemDetailsFactory,
            found => Ok(StockAlertResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }
}
