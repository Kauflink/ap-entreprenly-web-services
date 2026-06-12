using System.Net.Mime;
using Entreprenly.WebServices.Iam.Application.QueryServices;
using Entreprenly.WebServices.Iam.Domain.Model.Queries;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Iam.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Role endpoints")]
public class RolesController(IRoleQueryService roleQueryService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Get all roles", "Get all roles.", OperationId = "GetAllRoles")]
    [SwaggerResponse(StatusCodes.Status200OK, "The roles were found", typeof(IEnumerable<RoleResource>))]
    public async Task<IActionResult> GetAllRoles(CancellationToken cancellationToken)
    {
        var roles = await roleQueryService.Handle(new GetAllRolesQuery(), cancellationToken);
        var roleResources = roles.Select(RoleResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(roleResources);
    }
}
