namespace Entreprenly.WebServices.Iam.Domain.Model.Commands;

/// <summary>
///     Command carrying the data required to change an authenticated user's password.
/// </summary>
public record ChangePasswordCommand(int UserId, string CurrentPassword, string NewPassword);
