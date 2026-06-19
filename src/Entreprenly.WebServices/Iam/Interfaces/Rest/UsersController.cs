using System.Net.Mime;
using Entreprenly.WebServices.Iam.Application.CommandServices;
using Entreprenly.WebServices.Iam.Application.QueryServices;
using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
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
    IUserCommandService userCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    private int CurrentUserId => (HttpContext.Items["User"] as User)?.Id ?? 0;

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

    [HttpPut("me/password")]
    [SwaggerOperation("Change password",
        "Change the authenticated user's password after verifying the current one.", OperationId = "ChangePassword")]
    [SwaggerResponse(StatusCodes.Status200OK, "The password was updated", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The current password is incorrect or the request is invalid")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordResource resource,
        CancellationToken cancellationToken)
    {
        var command = ChangePasswordCommandFromResourceAssembler.ToCommandFromResource(CurrentUserId, resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromChangeCredentialsResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedUser => Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(updatedUser))
        );
    }

    [HttpPut("me/email")]
    [SwaggerOperation("Change email",
        "Change the authenticated user's email. The existing JWT becomes stale and the user must sign in again.",
        OperationId = "ChangeEmail")]
    [SwaggerResponse(StatusCodes.Status200OK, "The email was updated", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The new email matches the current one or the request is invalid")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The email is already taken")]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailResource resource,
        CancellationToken cancellationToken)
    {
        var command = ChangeEmailCommandFromResourceAssembler.ToCommandFromResource(CurrentUserId, resource);
        var result = await userCommandService.Handle(command, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromChangeCredentialsResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            updatedUser => Ok(UserResourceFromEntityAssembler.ToResourceFromEntity(updatedUser))
        );
    }
}
