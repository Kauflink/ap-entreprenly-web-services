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
[Route("api/v1/inventory-weight-products")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("By-weight inventory product endpoints")]
public class WeightProductsController(
    IWeightProductCommandService weightProductCommandService,
    IWeightProductQueryService weightProductQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : InventoryControllerBase
{
    [HttpGet]
    [SwaggerOperation("List weight products", "Retrieves every weight product owned by the authenticated account.",
        OperationId = "GetAllWeightProducts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Weight products found", typeof(IEnumerable<WeightProductResource>))]
    public async Task<IActionResult> GetAllWeightProducts(CancellationToken cancellationToken)
    {
        var products =
            await weightProductQueryService.Handle(new GetAllWeightProductsQuery(OwnerEmail), cancellationToken);
        return Ok(products.Select(WeightProductResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{weightProductId:int}")]
    [SwaggerOperation("Get weight product by id", "Retrieves a weight product by its identifier.",
        OperationId = "GetWeightProductById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Weight product found", typeof(WeightProductResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Weight product not found")]
    public async Task<IActionResult> GetWeightProductById(int weightProductId, CancellationToken cancellationToken)
    {
        var product = await weightProductQueryService.Handle(
            new GetWeightProductByIdQuery(OwnerEmail, weightProductId), cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromEntity(this, product,
            InventoryError.WeightProductNotFound, errorLocalizer, problemDetailsFactory,
            found => Ok(WeightProductResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    [HttpPost]
    [SwaggerOperation("Create weight product", "Registers a new product tracked and sold by weight.",
        OperationId = "CreateWeightProduct")]
    [SwaggerResponse(StatusCodes.Status201Created, "Weight product created", typeof(WeightProductResource))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "A weight product already exists with the QR code")]
    public async Task<IActionResult> CreateWeightProduct([FromBody] CreateWeightProductResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateWeightProductCommandFromResourceAssembler.ToCommandFromResource(OwnerEmail, resource);
        var result = await weightProductCommandService.Handle(command, cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetWeightProductById), new { weightProductId = created.Id },
                WeightProductResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }

    [HttpPut("{weightProductId:int}")]
    [SwaggerOperation("Update weight product", "Updates a weight product owned by the authenticated account.",
        OperationId = "UpdateWeightProduct")]
    [SwaggerResponse(StatusCodes.Status200OK, "Weight product updated", typeof(WeightProductResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Weight product not found")]
    public async Task<IActionResult> UpdateWeightProduct(int weightProductId,
        [FromBody] UpdateWeightProductResource resource, CancellationToken cancellationToken)
    {
        var command =
            UpdateWeightProductCommandFromResourceAssembler.ToCommandFromResource(OwnerEmail, weightProductId,
                resource);
        var result = await weightProductCommandService.Handle(command, cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            updated => Ok(WeightProductResourceFromEntityAssembler.ToResourceFromEntity(updated)));
    }

    [HttpDelete("{weightProductId:int}")]
    [SwaggerOperation("Delete weight product", "Deletes a weight product and its lots.",
        OperationId = "DeleteWeightProduct")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "Weight product deleted")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Weight product not found")]
    public async Task<IActionResult> DeleteWeightProduct(int weightProductId, CancellationToken cancellationToken)
    {
        var result = await weightProductCommandService.Handle(
            new DeleteWeightProductCommand(OwnerEmail, weightProductId), cancellationToken);
        return InventoryActionResultAssembler.ToActionResultFromResult(this, result, problemDetailsFactory,
            _ => NoContent());
    }
}
