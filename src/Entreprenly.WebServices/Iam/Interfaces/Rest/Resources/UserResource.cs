namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

public record UserResource(int Id, string Email, IEnumerable<string> Roles);
