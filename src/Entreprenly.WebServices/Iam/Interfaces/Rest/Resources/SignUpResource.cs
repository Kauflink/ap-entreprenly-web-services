using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

public record SignUpResource(
    [property: Required] [property: EmailAddress] [property: StringLength(120)]
    string Email,
    [property: Required] [property: StringLength(255, MinimumLength = 8)]
    string Password,
    [property: StringLength(80)] string? FirstName,
    [property: StringLength(80)] string? LastName,
    [property: StringLength(30)] string? Phone,
    [property: StringLength(60)] string? Timezone);
