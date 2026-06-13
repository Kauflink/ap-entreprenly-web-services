using Entreprenly.WebServices.Profiles.Domain.Model.Aggregates;
using Entreprenly.WebServices.Profiles.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;

namespace Entreprenly.WebServices.Profiles.Application.CommandServices;

/// <summary>
///     Handles profile-related commands.
/// </summary>
public interface IProfileCommandService
{
    Task<Result<Profile>> Handle(CreateProfileCommand command, CancellationToken cancellationToken);

    Task<Result<Profile>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken);

    Task<Result<Profile>> Handle(UpdatePreferencesCommand command, CancellationToken cancellationToken);

    Task<Result<Profile>> Handle(UpdateNotificationSettingsCommand command, CancellationToken cancellationToken);

    Task<Result<Profile>> Handle(UpdateProfilePlanCommand command, CancellationToken cancellationToken);
}
