using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

public record ChangePasswordResource(
    [Required] string CurrentPassword,
    [Required] [StringLength(255, MinimumLength = 8)] string NewPassword);
