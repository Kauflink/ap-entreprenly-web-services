namespace Entreprenly.WebServices.Iam.Domain.Model.Commands;

/// <summary>
///     Command carrying the data required to change an authenticated user's email (login identity).
/// </summary>
public record ChangeEmailCommand(int UserId, string NewEmail);
