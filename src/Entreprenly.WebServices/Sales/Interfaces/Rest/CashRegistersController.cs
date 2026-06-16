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
[SwaggerTag("Daily cash register endpoints")]
public class CashRegistersController(
    ICashRegisterCommandService cashRegisterCommandService,
    ICashRegisterQueryService cashRegisterQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost]
    [SwaggerOperation("Open a cash register", "Opens a cash register for a business day.",
        OperationId = "CreateCashRegister")]
    [SwaggerResponse(StatusCodes.Status201Created, "The cash register was opened", typeof(CashRegisterResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "A cash register already exists for the day")]
    public async Task<IActionResult> CreateCashRegister([FromBody] CreateCashRegisterResource resource,
        CancellationToken cancellationToken)
    {
        var ownerEmail = CurrentOwnerEmail();
        var command = CreateCashRegisterCommandFromResourceAssembler.ToCommandFromResource(ownerEmail, resource);
        var result = await cashRegisterCommandService.Handle(command, cancellationToken);
        return SalesActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetCashRegisters), null,
                CashRegisterResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }

    [HttpGet]
    [SwaggerOperation("List cash registers", "Retrieves cash registers, optionally filtered by business day.",
        OperationId = "GetCashRegisters")]
    [SwaggerResponse(StatusCodes.Status200OK, "The cash registers were found",
        typeof(IEnumerable<CashRegisterResource>))]
    public async Task<IActionResult> GetCashRegisters([FromQuery] DateOnly? date, CancellationToken cancellationToken)
    {
        var ownerEmail = CurrentOwnerEmail();
        if (date is not null)
        {
            var register = await cashRegisterQueryService.Handle(
                new GetCashRegisterByDateQuery(ownerEmail, date.Value), cancellationToken);
            var single = register is null
                ? Enumerable.Empty<CashRegisterResource>()
                : [CashRegisterResourceFromEntityAssembler.ToResourceFromEntity(register)];
            return Ok(single);
        }

        var registers = await cashRegisterQueryService.Handle(new GetAllCashRegistersQuery(ownerEmail),
            cancellationToken);
        return Ok(registers.Select(CashRegisterResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPut("{cashRegisterId:int}")]
    [SwaggerOperation("Update a cash register", "Updates the running totals and sale count of a cash register.",
        OperationId = "UpdateCashRegister")]
    [SwaggerResponse(StatusCodes.Status200OK, "The cash register was updated", typeof(CashRegisterResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid input data")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The cash register was not found")]
    public async Task<IActionResult> UpdateCashRegister(int cashRegisterId,
        [FromBody] UpdateCashRegisterResource resource, CancellationToken cancellationToken)
    {
        var ownerEmail = CurrentOwnerEmail();
        var command = UpdateCashRegisterCommandFromResourceAssembler.ToCommandFromResource(
            ownerEmail, cashRegisterId, resource);
        var result = await cashRegisterCommandService.Handle(command, cancellationToken);
        return SalesActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            updated => Ok(CashRegisterResourceFromEntityAssembler.ToResourceFromEntity(updated)));
    }

    private string CurrentOwnerEmail()
    {
        return (HttpContext.Items["User"] as User)?.Email ?? string.Empty;
    }
}
