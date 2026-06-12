namespace Entreprenly.WebServices.Profiles.Domain.Model.Commands;

/// <summary>
///     Command to create a profile for an existing user.
/// </summary>
public record CreateProfileCommand(
    int UserId,
    string FirstName,
    string LastName,
    string Role,
    string Plan,
    string? Phone,
    string? Timezone);
