using System.Net.Mime;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Inventory.Application.CommandServices;
using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model;
using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/inventory-unit-lots")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Per-unit inventory lot endpoints")]
public class UnitLotsController(
    IUnitLotCommandService unitLotCommandService,
    IUnitLotQueryService unitLotQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : InventoryControllerBase
{
    [HttpGet]
    [SwaggerOperation("List unit lots", "Retrieves every unit lot owned by the authenticated account.",
        OperationId = "GetAllUnitLots")]
    [SwaggerResponse(StatusCodes.Status200OK, "Unit lots found", typeof(IEnumerable<UnitLotResource>))]
    public async Task<IActionResult> GetAllUnitLots(CancellationToken cancellationToken)
    {
        var lots = await unitLotQueryService.Handle(new GetAllUnitLotsQuery(OwnerEmail), cancellationToken);
        return Ok(lots.Select(UnitLotResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{unitLotId:int}")]
    [SwaggerOperation("Get unit lot by id", "Retrieves a unit lot by its identifier.", OperationId = "GetUnitLotById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Unit lot found", typeof(UnitLotResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Unit lot not found")]
    public async Task<IActionResult> GetUnitLotById(int unitLotId, CancellationToken cancellationToken)
    {
        var lot = await unitLotQueryService.Handle(new GetUnitLotByIdQuery(OwnerEmail, unitLotId), cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromEntity(this, lot, InventoryError.UnitLotNotFound,
            errorLocalizer, problemDetailsFactory,
            found => Ok(UnitLotResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    [HttpPost]
    [SwaggerOperation("Create unit lot", "Registers a new stock batch for a per-unit product.",
        OperationId = "CreateUnitLot")]
    [SwaggerResponse(StatusCodes.Status201Created, "Unit lot created", typeof(UnitLotResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Referenced unit product not found")]
    public async Task<IActionResult> CreateUnitLot([FromBody] CreateUnitLotResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateUnitLotCommandFromResourceAssembler.ToCommandFromResource(OwnerEmail, resource);
        var result = await unitLotCommandService.Handle(command, cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetUnitLotById), new { unitLotId = created.Id },
                UnitLotResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }

    [HttpPut("{unitLotId:int}")]
    [SwaggerOperation("Update unit lot", "Updates a unit lot owned by the authenticated account.",
        OperationId = "UpdateUnitLot")]
    [SwaggerResponse(StatusCodes.Status200OK, "Unit lot updated", typeof(UnitLotResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Unit lot not found")]
    public async Task<IActionResult> UpdateUnitLot(int unitLotId, [FromBody] UpdateUnitLotResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateUnitLotCommandFromResourceAssembler.ToCommandFromResource(OwnerEmail, unitLotId, resource);
        var result = await unitLotCommandService.Handle(command, cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            updated => Ok(UnitLotResourceFromEntityAssembler.ToResourceFromEntity(updated)));
    }

    [HttpDelete("{unitLotId:int}")]
    [SwaggerOperation("Delete unit lot", "Deletes a unit lot owned by the authenticated account.",
        OperationId = "DeleteUnitLot")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Unit lot deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Unit lot not found")]
    public async Task<IActionResult> DeleteUnitLot(int unitLotId, CancellationToken cancellationToken)
    {
        var result = await unitLotCommandService.Handle(new DeleteUnitLotCommand(OwnerEmail, unitLotId),
            cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            _ => NoContent());
    }
}
