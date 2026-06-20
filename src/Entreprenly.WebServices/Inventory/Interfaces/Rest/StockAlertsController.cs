using System.Net.Mime;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Profiles.Application.QueryServices;
using Entreprenly.WebServices.Profiles.Domain.Model.Queries;
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
    IProfileQueryService profileQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : InventoryControllerBase
{
    [HttpGet]
    [SwaggerOperation("List stock alerts",
        "Computes and retrieves every currently raised stock alert, ordered by descending urgency. " +
        "Returns an empty list when the account disabled stock alert notifications.",
        OperationId = "GetAllStockAlerts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Stock alerts computed", typeof(IEnumerable<StockAlertResource>))]
    public async Task<IActionResult> GetAllStockAlerts(CancellationToken cancellationToken)
    {
        if (!await StockAlertsEnabledAsync(cancellationToken))
            return Ok(Enumerable.Empty<StockAlertResource>());

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
        var alert = !await StockAlertsEnabledAsync(cancellationToken)
            ? null
            : await stockAlertQueryService.Handle(new GetStockAlertByIdQuery(OwnerEmail, stockAlertId),
                cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromEntity(this, alert, InventoryError.UnitLotNotFound,
            errorLocalizer, problemDetailsFactory,
            found => Ok(StockAlertResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    /// <summary>
    ///     Reads the account's notification preference. Stock alerts are enabled by default and only
    ///     suppressed when the profile explicitly turned them off.
    /// </summary>
    private async Task<bool> StockAlertsEnabledAsync(CancellationToken cancellationToken)
    {
        var profile = await profileQueryService.Handle(new GetProfileByUserIdQuery(OwnerUserId), cancellationToken);
        return profile?.NotificationSettings.StockAlerts ?? true;
    }
}
