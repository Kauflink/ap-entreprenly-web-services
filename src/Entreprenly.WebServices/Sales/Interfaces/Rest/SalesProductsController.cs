using System.Net.Mime;
using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Inventory.Interfaces.Acl;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/sales-products")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Products available to sell at the point of sale")]
public class SalesProductsController(IInventoryContextFacade inventoryContextFacade) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("List sellable products",
        "Retrieves the authenticated seller's catalog with each product's currently available stock, " +
        "computed by the Inventory context, ready to be sold at the point of sale.",
        OperationId = "GetSalesProducts")]
    [SwaggerResponse(StatusCodes.Status200OK, "Sellable products found", typeof(IEnumerable<SalesProductResource>))]
    public async Task<IActionResult> GetSalesProducts(CancellationToken cancellationToken)
    {
        var catalog = await inventoryContextFacade.FetchCatalogByOwnerAsync(CurrentOwnerEmail(), cancellationToken);
        return Ok(catalog.Select(SalesProductResourceFromCatalogItemAssembler.ToResourceFromCatalogItem));
    }

    private string CurrentOwnerEmail()
    {
        return (HttpContext.Items["User"] as User)?.Email ?? string.Empty;
    }
}
