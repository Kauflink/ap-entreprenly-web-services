namespace Entreprenly.WebServices.Profiles.Domain.Model.Commands;

/// <summary>
///     Command to update the display plan of the profile that belongs to a user.
/// </summary>
public record UpdateProfilePlanCommand(int UserId, string Plan);
