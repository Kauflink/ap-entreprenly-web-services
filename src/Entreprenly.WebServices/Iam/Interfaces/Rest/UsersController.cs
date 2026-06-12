using System.Net.Mime;
using Entreprenly.WebServices.Iam.Application.QueryServices;
using Entreprenly.WebServices.Iam.Domain.Model.Queries;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Iam.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available User endpoints")]
public class UsersController(
    IUserQueryService userQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet("{id:int}")]
    [SwaggerOperation("Get a user by its id", "Get a user by its id.", OperationId = "GetUserById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was found", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The user was not found")]
    public async Task<IActionResult> GetUserById(int id, CancellationToken cancellationToken)
    {
        var user = await userQueryService.Handle(new GetUserByIdQuery(id), cancellationToken);

        return IamActionResultAssembler.ToActionResultFromGetUserByIdResult(
            this,
            user,
            errorLocalizer,
            problemDetailsFactory,
            foundUser => Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(foundUser))
        );
    }

    [HttpGet]
    [SwaggerOperation("Get all users", "Get all users.", OperationId = "GetAllUsers")]
    [SwaggerResponse(StatusCodes.Status200OK, "The users were found", typeof(IEnumerable<UserResource>))]
    public async Task<IActionResult> GetAllUsers(CancellationToken cancellationToken)
    {
        var users = await userQueryService.Handle(new GetAllUsersQuery(), cancellationToken);
        var userResources = users.Select(UserResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(userResources);
    }
}
