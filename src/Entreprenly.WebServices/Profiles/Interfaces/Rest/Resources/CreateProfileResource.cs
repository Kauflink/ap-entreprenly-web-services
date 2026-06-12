namespace Entreprenly.WebServices.Profiles.Interfaces.Rest.Resources;

public record CreateProfileResource(
    int UserId,
    string FirstName,
    string LastName,
    string Role,
    string Plan,
    string? Phone,
    string? Timezone);
