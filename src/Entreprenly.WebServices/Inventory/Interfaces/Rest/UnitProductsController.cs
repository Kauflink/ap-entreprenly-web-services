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
[Route("api/v1/inventory-unit-products")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Per-unit inventory product endpoints")]
public class UnitProductsController(
    IUnitProductCommandService unitProductCommandService,
    IUnitProductQueryService unitProductQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : InventoryControllerBase
{
    [HttpGet]
    [SwaggerOperation("List unit products", "Retrieves every unit product owned by the authenticated account.",
        OperationId = "GetAllUnitProducts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Unit products found", typeof(IEnumerable<UnitProductResource>))]
    public async Task<IActionResult> GetAllUnitProducts(CancellationToken cancellationToken)
    {
        var products = await unitProductQueryService.Handle(new GetAllUnitProductsQuery(OwnerEmail), cancellationToken);
        return Ok(products.Select(UnitProductResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{unitProductId:int}")]
    [SwaggerOperation("Get unit product by id", "Retrieves a unit product by its identifier.",
        OperationId = "GetUnitProductById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Unit product found", typeof(UnitProductResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Unit product not found")]
    public async Task<IActionResult> GetUnitProductById(int unitProductId, CancellationToken cancellationToken)
    {
        var product = await unitProductQueryService.Handle(new GetUnitProductByIdQuery(OwnerEmail, unitProductId),
            cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromEntity(this, product,
            InventoryError.UnitProductNotFound, errorLocalizer, problemDetailsFactory,
            found => Ok(UnitProductResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    [HttpPost]
    [SwaggerOperation("Create unit product", "Registers a new product tracked and sold per unit.",
        OperationId = "CreateUnitProduct")]
    [SwaggerResponse(StatusCodes.Status201Created, "Unit product created", typeof(UnitProductResource))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "A unit product already exists with the QR code")]
    public async Task<IActionResult> CreateUnitProduct([FromBody] CreateUnitProductResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateUnitProductCommandFromResourceAssembler.ToCommandFromResource(OwnerEmail, resource);
        var result = await unitProductCommandService.Handle(command, cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetUnitProductById), new { unitProductId = created.Id },
                UnitProductResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }

    [HttpPut("{unitProductId:int}")]
    [SwaggerOperation("Update unit product", "Updates a unit product owned by the authenticated account.",
        OperationId = "UpdateUnitProduct")]
    [SwaggerResponse(StatusCodes.Status200OK, "Unit product updated", typeof(UnitProductResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Unit product not found")]
    public async Task<IActionResult> UpdateUnitProduct(int unitProductId,
        [FromBody] UpdateUnitProductResource resource, CancellationToken cancellationToken)
    {
        var command =
            UpdateUnitProductCommandFromResourceAssembler.ToCommandFromResource(OwnerEmail, unitProductId, resource);
        var result = await unitProductCommandService.Handle(command, cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            updated => Ok(UnitProductResourceFromEntityAssembler.ToResourceFromEntity(updated)));
    }

    [HttpDelete("{unitProductId:int}")]
    [SwaggerOperation("Delete unit product", "Deletes a unit product and its lots.",
        OperationId = "DeleteUnitProduct")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Unit product deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Unit product not found")]
    public async Task<IActionResult> DeleteUnitProduct(int unitProductId, CancellationToken cancellationToken)
    {
        var result = await unitProductCommandService.Handle(new DeleteUnitProductCommand(OwnerEmail, unitProductId),
            cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            _ => NoContent());
    }
}
