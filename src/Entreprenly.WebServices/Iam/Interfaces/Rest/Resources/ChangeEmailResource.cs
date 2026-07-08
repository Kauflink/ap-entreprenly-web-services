using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

public record ChangeEmailResource(
    [Required] [EmailAddress] [StringLength(120)] string Email);
