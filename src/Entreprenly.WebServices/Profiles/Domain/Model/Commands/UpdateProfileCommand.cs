namespace Entreprenly.WebServices.Profiles.Domain.Model.Commands;

/// <summary>
///     Command to update the editable fields of a profile.
/// </summary>
public record UpdateProfileCommand(
    int ProfileId,
    string FirstName,
    string LastName,
    string? Phone,
    string? AvatarUrl);
