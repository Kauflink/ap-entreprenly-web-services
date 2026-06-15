using System.Net.Mime;
using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Sales.Application.CommandServices;
using Entreprenly.WebServices.Sales.Application.QueryServices;
using Entreprenly.WebServices.Sales.Domain.Model.Queries;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Point-of-sale transaction endpoints")]
public class SalesController(
    ISaleCommandService saleCommandService,
    ISaleQueryService saleQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [SwaggerOperation("Register a sale",
        "Registers a new point-of-sale transaction. The server recomputes the total and line subtotals.",
        OperationId = "CreateSale")]
    [SwaggerResponse(StatusCodes.Status201Created, "The sale was registered", typeof(SaleResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data")]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleResource resource,
        CancellationToken cancellationToken)
    {
        var ownerEmail = CurrentOwnerEmail();
        var command = CreateSaleCommandFromResourceAssembler.ToCommandFromResource(ownerEmail, resource);
        var result = await saleCommandService.Handle(command, cancellationToken);
        return SalesActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetSaleById), new { saleId = created.Id },
                SaleResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }

    [HttpGet]
    [SwaggerOperation("List sales", "Retrieves every sale registered by the authenticated account.",
        OperationId = "GetAllSales")]
    [SwaggerResponse(StatusCodes.Status200OK, "The sales were found", typeof(IEnumerable<SaleResource>))]
    public async Task<IActionResult> GetAllSales(CancellationToken cancellationToken)
    {
        var sales = await saleQueryService.Handle(new GetAllSalesQuery(CurrentOwnerEmail()), cancellationToken);
        return Ok(sales.Select(SaleResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{saleId:int}")]
    [SwaggerOperation("Get sale by id", "Retrieves a sale by its unique identifier.", OperationId = "GetSaleById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The sale was found", typeof(SaleResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The sale was not found")]
    public async Task<IActionResult> GetSaleById(int saleId, CancellationToken cancellationToken)
    {
        var sale = await saleQueryService.Handle(new GetSaleByIdQuery(CurrentOwnerEmail(), saleId), cancellationToken);
        return sale is null
            ? NotFound()
            : Ok(SaleResourceFromEntityAssembler.ToResourceFromEntity(sale));
    }

    private string CurrentOwnerEmail()
    {
        return (HttpContext.Items["User"] as User)?.Email ?? string.Empty;
    }
}
