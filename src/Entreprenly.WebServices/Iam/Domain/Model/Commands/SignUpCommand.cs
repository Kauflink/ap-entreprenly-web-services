namespace Entreprenly.WebServices.Iam.Domain.Model.Commands;

/// <summary>
///     Command to register a new user. The default role is assigned by the application service;
///     the remaining fields are optional profile bootstrap data forwarded to the Profiles context.
/// </summary>
public record SignUpCommand(
    string Email,
    string Password,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? Timezone);
