namespace Entreprenly.WebServices.Profiles.Domain.Model.Commands;

/// <summary>
///     Command to replace a profile's preferences.
/// </summary>
public record UpdatePreferencesCommand(
    int ProfileId,
    string Language,
    string Timezone,
    string Theme,
    string Currency);
