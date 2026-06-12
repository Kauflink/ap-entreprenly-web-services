using Entreprenly.WebServices.Profiles.Domain.Model;
using Entreprenly.WebServices.Profiles.Domain.Model.Aggregates;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Profiles.Interfaces.Rest.Transform;

public static class ProfilesActionResultAssembler
{
    private static int ToStatusCodeFromProfilesError(ProfilesError error)
    {
        return error switch
        {
            ProfilesError.ProfileNotFound => StatusCodes.Status404NotFound,
            ProfilesError.ProfileAlreadyExists => StatusCodes.Status409Conflict,
            ProfilesError.InvalidProfileData => StatusCodes.Status400BadRequest,
            ProfilesError.OperationCancelled => StatusCodes.Status409Conflict,
            ProfilesError.DatabaseError => StatusCodes.Status500InternalServerError,
            ProfilesError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResultFromResult(
        ControllerBase controller,
        Result<Profile> result,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Profile, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromProfilesError((ProfilesError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromGetProfileResult(
        ControllerBase controller,
        Profile? profile,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Profile, IActionResult> successAction)
    {
        if (profile is null)
            return problemDetailsFactory.CreateProblemDetails(
                controller,
                ToStatusCodeFromProfilesError(ProfilesError.ProfileNotFound),
                ProfilesError.ProfileNotFound,
                errorLocalizer[nameof(ProfilesError.ProfileNotFound)]
            );
        return successAction(profile);
    }
}
