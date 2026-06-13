namespace Entreprenly.WebServices.Profiles.Interfaces.Rest.Resources;

public record UpdateProfileResource(string FirstName, string LastName, string? Phone, string? AvatarUrl);
