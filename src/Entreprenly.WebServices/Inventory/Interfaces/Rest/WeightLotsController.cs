using System.Net.Mime;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Inventory.Application.CommandServices;
using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model;
using Entreprenly.WebServices.Inventory.Domain.Model.Commands;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Inventory.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/inventory-weight-lots")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("By-weight inventory lot endpoints")]
public class WeightLotsController(
    IWeightLotCommandService weightLotCommandService,
    IWeightLotQueryService weightLotQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : InventoryControllerBase
{
    [HttpGet]
    [SwaggerOperation("List weight lots", "Retrieves every weight lot owned by the authenticated account.",
        OperationId = "GetAllWeightLots")]
    [SwaggerResponse(StatusCodes.Status200OK, "Weight lots found", typeof(IEnumerable<WeightLotResource>))]
    public async Task<IActionResult> GetAllWeightLots(CancellationToken cancellationToken)
    {
        var lots = await weightLotQueryService.Handle(new GetAllWeightLotsQuery(OwnerEmail), cancellationToken);
        return Ok(lots.Select(WeightLotResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{weightLotId:int}")]
    [SwaggerOperation("Get weight lot by id", "Retrieves a weight lot by its identifier.",
        OperationId = "GetWeightLotById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Weight lot found", typeof(WeightLotResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Weight lot not found")]
    public async Task<IActionResult> GetWeightLotById(int weightLotId, CancellationToken cancellationToken)
    {
        var lot = await weightLotQueryService.Handle(new GetWeightLotByIdQuery(OwnerEmail, weightLotId),
            cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromEntity(this, lot, InventoryError.WeightLotNotFound,
            errorLocalizer, problemDetailsFactory,
            found => Ok(WeightLotResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    [HttpPost]
    [SwaggerOperation("Create weight lot", "Registers a new stock batch for a by-weight product.",
        OperationId = "CreateWeightLot")]
    [SwaggerResponse(StatusCodes.Status201Created, "Weight lot created", typeof(WeightLotResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Referenced weight product not found")]
    public async Task<IActionResult> CreateWeightLot([FromBody] CreateWeightLotResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateWeightLotCommandFromResourceAssembler.ToCommandFromResource(OwnerEmail, resource);
        var result = await weightLotCommandService.Handle(command, cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetWeightLotById), new { weightLotId = created.Id },
                WeightLotResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }

    [HttpPut("{weightLotId:int}")]
    [SwaggerOperation("Update weight lot", "Updates a weight lot owned by the authenticated account.",
        OperationId = "UpdateWeightLot")]
    [SwaggerResponse(StatusCodes.Status200OK, "Weight lot updated", typeof(WeightLotResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Weight lot not found")]
    public async Task<IActionResult> UpdateWeightLot(int weightLotId, [FromBody] UpdateWeightLotResource resource,
        CancellationToken cancellationToken)
    {
        var command =
            UpdateWeightLotCommandFromResourceAssembler.ToCommandFromResource(OwnerEmail, weightLotId, resource);
        var result = await weightLotCommandService.Handle(command, cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            updated => Ok(WeightLotResourceFromEntityAssembler.ToResourceFromEntity(updated)));
    }

    [HttpDelete("{weightLotId:int}")]
    [SwaggerOperation("Delete weight lot", "Deletes a weight lot owned by the authenticated account.",
        OperationId = "DeleteWeightLot")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Weight lot deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Weight lot not found")]
    public async Task<IActionResult> DeleteWeightLot(int weightLotId, CancellationToken cancellationToken)
    {
        var result = await weightLotCommandService.Handle(new DeleteWeightLotCommand(OwnerEmail, weightLotId),
            cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            _ => NoContent());
    }
}
