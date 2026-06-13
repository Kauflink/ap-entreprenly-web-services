using System.Net.Mime;
using Entreprenly.WebServices.Iam.Application.CommandServices;
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
[SwaggerTag("Available Authentication endpoints")]
public class AuthenticationController(
    IUserCommandService userCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpPost("sign-in")]
    [AllowAnonymous]
    [SwaggerOperation("Sign in", "Authenticate a user and issue a JWT token.", OperationId = "SignIn")]
    [SwaggerResponse(StatusCodes.Status200OK, "The user was authenticated", typeof(AuthenticatedUserResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid email or password")]
    public async Task<IActionResult> SignIn([FromBody] SignInResource signInResource,
        CancellationToken cancellationToken)
    {
        var signInCommand = SignInCommandFromResourceAssembler.ToCommandFromResource(signInResource);
        var result = await userCommandService.Handle(signInCommand, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromSignInResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            userAndToken =>
                Ok(AuthenticatedUserResourceFromEntityAssembler.ToResourceFromEntity(userAndToken.user,
                    userAndToken.token))
        );
    }

    [HttpPost("sign-up")]
    [AllowAnonymous]
    [SwaggerOperation("Sign up", "Register a new user.", OperationId = "SignUp")]
    [SwaggerResponse(StatusCodes.Status201Created, "The user was created successfully", typeof(UserResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "The user was not created")]
    [SwaggerResponse(StatusCodes.Status409Conflict, "The email is already taken")]
    public async Task<IActionResult> SignUp([FromBody] SignUpResource signUpResource,
        CancellationToken cancellationToken)
    {
        var signUpCommand = SignUpCommandFromResourceAssembler.ToCommandFromResource(signUpResource);
        var result = await userCommandService.Handle(signUpCommand, cancellationToken);

        return IamActionResultAssembler.ToActionResultFromSignUpResult(
            this,
            result,
            errorLocalizer,
            problemDetailsFactory,
            createdUser => StatusCode(StatusCodes.Status201Created,
                UserResourceFromEntityAssembler.ToResourceFromEntity(createdUser))
        );
    }
}
