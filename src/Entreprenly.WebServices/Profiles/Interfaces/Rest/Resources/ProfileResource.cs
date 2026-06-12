namespace Entreprenly.WebServices.Profiles.Interfaces.Rest.Resources;

public record ProfileResource(
    int Id,
    int UserId,
    string FirstName,
    string LastName,
    string? Phone,
    string? AvatarUrl,
    string Role,
    string Plan,
    PreferencesResource Preferences,
    NotificationSettingsResource NotificationSettings);
