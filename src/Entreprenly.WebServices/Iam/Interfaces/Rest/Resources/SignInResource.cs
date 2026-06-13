using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

public record SignInResource(
    [Required] [EmailAddress] string Email,
    [Required] string Password);
