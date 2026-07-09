using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

public record SignUpResource(
    [Required] [EmailAddress] [StringLength(120)] string Email,
    [Required] [StringLength(255, MinimumLength = 8)] string Password,
    [StringLength(80)] string? FirstName,
    [StringLength(80)] string? LastName,
    [StringLength(30)] string? Phone,
    [StringLength(60)] string? Timezone);
