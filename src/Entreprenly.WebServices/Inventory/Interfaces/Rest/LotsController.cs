using System.Net.Mime;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest;

/// <summary>
///     Exposes the combined (unit + weight) lot read view. Lots are created and mutated through
///     their type-specific endpoints; this controller only provides a unified read-only listing.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/inventory-lots")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Combined inventory lot read endpoints")]
public class LotsController(
    ILotQueryService lotQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : InventoryControllerBase
{
    [HttpGet]
    [SwaggerOperation("List all lots", "Retrieves every registered lot (unit and weight) in a combined view.",
        OperationId = "GetAllLots")]
    [SwaggerResponse(StatusCodes.Status200OK, "Lots found", typeof(IEnumerable<LotResource>))]
    public async Task<IActionResult> GetAllLots(CancellationToken cancellationToken)
    {
        var lots = await lotQueryService.Handle(new GetAllLotsQuery(OwnerEmail), cancellationToken);
        return Ok(lots.Select(LotResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{lotId:int}")]
    [SwaggerOperation("Get lot by id", "Retrieves a single lot by its identifier from the combined view.",
        OperationId = "GetLotById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Lot found", typeof(LotResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Lot not found")]
    public async Task<IActionResult> GetLotById(int lotId, CancellationToken cancellationToken)
    {
        var lot = await lotQueryService.Handle(new GetLotByIdQuery(OwnerEmail, lotId), cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromEntity(this, lot, InventoryError.UnitLotNotFound,
            errorLocalizer, problemDetailsFactory,
            found => Ok(LotResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }
}
