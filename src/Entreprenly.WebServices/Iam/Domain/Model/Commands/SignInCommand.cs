namespace Entreprenly.WebServices.Iam.Domain.Model.Commands;

/// <summary>
///     Command carrying the credentials used to authenticate a user.
/// </summary>
public record SignInCommand(string Email, string Password);
