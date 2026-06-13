namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

public record SignUpResource(
    string Email,
    string Password,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? Timezone);
