using Entreprenly.WebServices.Iam.Domain.Model;
using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Transform;

public static class IamActionResultAssembler
{
    private static int ToStatusCodeFromIamError(IamError error)
    {
        return error switch
        {
            IamError.UserNotFound => StatusCodes.Status404NotFound,
            IamError.InvalidCredentials => StatusCodes.Status400BadRequest,
            IamError.CurrentPasswordIncorrect => StatusCodes.Status400BadRequest,
            IamError.EmailMatchesCurrent => StatusCodes.Status400BadRequest,
            IamError.EmailAlreadyTaken => StatusCodes.Status409Conflict,
            IamError.RoleNotFound => StatusCodes.Status404NotFound,
            IamError.OperationCancelled => StatusCodes.Status409Conflict,
            IamError.DatabaseError => StatusCodes.Status500InternalServerError,
            IamError.InternalServerError => StatusCodes.Status500InternalServerError,
            IamError.ExternalServiceError => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResultFromSignInResult(
        ControllerBase controller,
        Result<(User user, string token)> result,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<(User user, string token), IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromIamError((IamError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromSignUpResult(
        ControllerBase controller,
        Result<User> result,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<User, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromIamError((IamError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromChangeCredentialsResult(
        ControllerBase controller,
        Result<User> result,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<User, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromIamError((IamError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromGetUserByIdResult(
        ControllerBase controller,
        User? user,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<User, IActionResult> successAction)
    {
        if (user is null)
            return problemDetailsFactory.CreateProblemDetails(
                controller,
                ToStatusCodeFromIamError(IamError.UserNotFound),
                IamError.UserNotFound,
                errorLocalizer[nameof(IamError.UserNotFound)]
            );
        return successAction(user);
    }
}
