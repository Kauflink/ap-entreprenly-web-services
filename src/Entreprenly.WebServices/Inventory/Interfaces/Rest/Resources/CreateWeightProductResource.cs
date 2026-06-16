using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to create a weight product.
/// </summary>
public record CreateWeightProductResource(
    [Required] string Name,
    string? Description,
    string? CodeQr,
    [Range(0, double.MaxValue)] double PricePerKg);
