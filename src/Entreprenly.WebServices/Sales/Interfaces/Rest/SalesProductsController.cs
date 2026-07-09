using System.Net.Mime;
using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Inventory.Interfaces.Acl;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest;

/// <summary>
///     REST controller that exposes the sellable product catalog for the point-of-sale view.
///     The catalog (products with price and already-computed stock) is read from the Inventory
///     bounded context through its ACL facade, so the point-of-sale client gets everything it
///     needs in a single request without knowing about products or lots separately.
/// </summary>
[Authorize]
[ApiController]
[Route("api/v1/sales-products")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Products available to sell at the point of sale")]
public class SalesProductsController(IInventoryContextFacade inventoryContextFacade) : ControllerBase
{
    /// <summary>
    ///     Returns the authenticated seller's catalog, each product carrying the stock currently
    ///     available (computed by the Inventory context) and ready to be sold.
    /// </summary>
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

    /// <summary>Resolves the authenticated account's email from the request context.</summary>
    private string CurrentOwnerEmail()
    {
        return (HttpContext.Items["User"] as User)?.Email ?? string.Empty;
    }
}
