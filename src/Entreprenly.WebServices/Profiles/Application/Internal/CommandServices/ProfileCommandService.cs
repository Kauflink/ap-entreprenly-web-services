using Entreprenly.WebServices.Profiles.Application.CommandServices;
using Entreprenly.WebServices.Profiles.Domain.Model;
using Entreprenly.WebServices.Profiles.Domain.Model.Aggregates;
using Entreprenly.WebServices.Profiles.Domain.Model.Commands;
using Entreprenly.WebServices.Profiles.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Profiles.Domain.Repositories;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Profiles.Application.Internal.CommandServices;

/// <summary>
///     Handles profile commands.
/// </summary>
public class ProfileCommandService(
    IProfileRepository profileRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IProfileCommandService
{
    public async Task<Result<Profile>> Handle(CreateProfileCommand command, CancellationToken cancellationToken)
    {
        if (await profileRepository.ExistsByUserIdAsync(command.UserId, cancellationToken))
            return Result<Profile>.Failure(ProfilesError.ProfileAlreadyExists,
                localizer[nameof(ProfilesError.ProfileAlreadyExists)]);

        var profile = new Profile(command.UserId, command.FirstName, command.LastName, command.Role, command.Plan,
            command.Phone, command.Timezone);

        await profileRepository.AddAsync(profile, cancellationToken);
        return await CompleteAsync(profile, cancellationToken);
    }

    public async Task<Result<Profile>> Handle(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.FindByIdAsync(command.ProfileId, cancellationToken);
        if (profile is null)
            return Result<Profile>.Failure(ProfilesError.ProfileNotFound,
                localizer[nameof(ProfilesError.ProfileNotFound)]);

        profile.UpdateProfile(command.FirstName, command.LastName, command.Phone, command.AvatarUrl);
        profileRepository.Update(profile);
        return await CompleteAsync(profile, cancellationToken);
    }

    public async Task<Result<Profile>> Handle(UpdatePreferencesCommand command, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.FindByIdAsync(command.ProfileId, cancellationToken);
        if (profile is null)
            return Result<Profile>.Failure(ProfilesError.ProfileNotFound,
                localizer[nameof(ProfilesError.ProfileNotFound)]);

        profile.UpdatePreferences(new Preferences(command.Language, command.Timezone, command.Theme, command.Currency));
        profileRepository.Update(profile);
        return await CompleteAsync(profile, cancellationToken);
    }

    public async Task<Result<Profile>> Handle(UpdateNotificationSettingsCommand command,
        CancellationToken cancellationToken)
    {
        var profile = await profileRepository.FindByIdAsync(command.ProfileId, cancellationToken);
        if (profile is null)
            return Result<Profile>.Failure(ProfilesError.ProfileNotFound,
                localizer[nameof(ProfilesError.ProfileNotFound)]);

        profile.UpdateNotificationSettings(
            new NotificationSettings(command.StockAlerts, command.PaymentAlerts, command.ChatbotMessages));
        profileRepository.Update(profile);
        return await CompleteAsync(profile, cancellationToken);
    }

    public async Task<Result<Profile>> Handle(UpdateProfilePlanCommand command, CancellationToken cancellationToken)
    {
        var profile = await profileRepository.FindByUserIdAsync(command.UserId, cancellationToken);
        if (profile is null)
            return Result<Profile>.Failure(ProfilesError.ProfileNotFound,
                localizer[nameof(ProfilesError.ProfileNotFound)]);

        profile.ChangePlan(command.Plan);
        profileRepository.Update(profile);
        return await CompleteAsync(profile, cancellationToken);
    }

    private async Task<Result<Profile>> CompleteAsync(Profile profile, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Profile>.Success(profile);
        }
        catch (OperationCanceledException)
        {
            return Result<Profile>.Failure(ProfilesError.OperationCancelled,
                localizer[nameof(ProfilesError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Profile>.Failure(ProfilesError.DatabaseError,
                localizer[nameof(ProfilesError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Profile>.Failure(ProfilesError.InternalServerError,
                localizer[nameof(ProfilesError.InternalServerError)]);
        }
    }
}
