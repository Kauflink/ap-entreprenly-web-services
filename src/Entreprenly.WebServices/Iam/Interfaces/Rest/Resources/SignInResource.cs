using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

public record SignInResource(
    [property: Required] [property: EmailAddress] string Email,
    [property: Required] string Password);
