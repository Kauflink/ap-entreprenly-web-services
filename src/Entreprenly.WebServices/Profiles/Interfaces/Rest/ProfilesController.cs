using System.Net.Mime;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Profiles.Application.CommandServices;
using Entreprenly.WebServices.Profiles.Application.QueryServices;
using Entreprenly.WebServices.Profiles.Domain.Model.Queries;
using Entreprenly.WebServices.Profiles.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Profiles.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Profiles.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Profile endpoints")]
public class ProfilesController(
    IProfileCommandService profileCommandService,
    IProfileQueryService profileQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Get all profiles", "Get all profiles.", OperationId = "GetAllProfiles")]
    [SwaggerResponse(StatusCodes.Status200OK, "The profiles were found", typeof(IEnumerable<ProfileResource>))]
    public async Task<IActionResult> GetAllProfiles(CancellationToken cancellationToken)
    {
        var profiles = await profileQueryService.Handle(new GetAllProfilesQuery(), cancellationToken);
        return Ok(profiles.Select(ProfileResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{profileId:int}")]
    [SwaggerOperation("Get profile by id", "Get a profile by its identifier.", OperationId = "GetProfileById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The profile was found", typeof(ProfileResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The profile was not found")]
    public async Task<IActionResult> GetProfileById(int profileId, CancellationToken cancellationToken)
    {
        var profile = await profileQueryService.Handle(new GetProfileByIdQuery(profileId), cancellationToken);
        return ProfilesActionResultAssembler.ToActionResultFromGetProfileResult(
            this, profile, errorLocalizer, problemDetailsFactory,
            found => Ok(ProfileResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    [HttpGet("by-user/{userId:int}")]
    [SwaggerOperation("Get profile by user id", "Get a profile by its user identifier.",
        OperationId = "GetProfileByUserId")]
    [SwaggerResponse(StatusCodes.Status200OK, "The profile was found", typeof(ProfileResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The profile was not found")]
    public async Task<IActionResult> GetProfileByUserId(int userId, CancellationToken cancellationToken)
    {
        var profile = await profileQueryService.Handle(new GetProfileByUserIdQuery(userId), cancellationToken);
        return ProfilesActionResultAssembler.ToActionResultFromGetProfileResult(
            this, profile, errorLocalizer, problemDetailsFactory,
            found => Ok(ProfileResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    [HttpPost]
    [SwaggerOperation("Create profile", "Create a new profile.", OperationId = "CreateProfile")]
    [SwaggerResponse(StatusCodes.Status201Created, "The profile was created", typeof(ProfileResource))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "A profile already exists for the user")]
    public async Task<IActionResult> CreateProfile([FromBody] CreateProfileResource resource,
        CancellationToken cancellationToken)
    {
        var command = CreateProfileCommandFromResourceAssembler.ToCommandFromResource(resource);
        var result = await profileCommandService.Handle(command, cancellationToken);
        return ProfilesActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetProfileById), new { profileId = created.Id },
                ProfileResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }

    [HttpPut("{profileId:int}")]
    [SwaggerOperation("Update profile", "Update the editable fields of a profile.", OperationId = "UpdateProfile")]
    [SwaggerResponse(StatusCodes.Status200OK, "The profile was updated", typeof(ProfileResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The profile was not found")]
    public async Task<IActionResult> UpdateProfile(int profileId, [FromBody] UpdateProfileResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdateProfileCommandFromResourceAssembler.ToCommandFromResource(profileId, resource);
        var result = await profileCommandService.Handle(command, cancellationToken);
        return ProfilesActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            updated => Ok(ProfileResourceFromEntityAssembler.ToResourceFromEntity(updated)));
    }

    [HttpPut("{profileId:int}/preferences")]
    [SwaggerOperation("Update preferences", "Replace a profile's preferences.", OperationId = "UpdatePreferences")]
    [SwaggerResponse(StatusCodes.Status200OK, "The preferences were updated", typeof(ProfileResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The profile was not found")]
    public async Task<IActionResult> UpdatePreferences(int profileId, [FromBody] UpdatePreferencesResource resource,
        CancellationToken cancellationToken)
    {
        var command = UpdatePreferencesCommandFromResourceAssembler.ToCommandFromResource(profileId, resource);
        var result = await profileCommandService.Handle(command, cancellationToken);
        return ProfilesActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            updated => Ok(ProfileResourceFromEntityAssembler.ToResourceFromEntity(updated)));
    }

    [HttpPut("{profileId:int}/notification-settings")]
    [SwaggerOperation("Update notification settings", "Replace a profile's notification settings.",
        OperationId = "UpdateNotificationSettings")]
    [SwaggerResponse(StatusCodes.Status200OK, "The notification settings were updated", typeof(ProfileResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The profile was not found")]
    public async Task<IActionResult> UpdateNotificationSettings(int profileId,
        [FromBody] UpdateNotificationSettingsResource resource, CancellationToken cancellationToken)
    {
        var command = UpdateNotificationSettingsCommandFromResourceAssembler.ToCommandFromResource(profileId, resource);
        var result = await profileCommandService.Handle(command, cancellationToken);
        return ProfilesActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            updated => Ok(ProfileResourceFromEntityAssembler.ToResourceFromEntity(updated)));
    }
}
