using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to update a weight product.
/// </summary>
public record UpdateWeightProductResource(
    [Required] string Name,
    string? Description,
    string? CodeQr,
    [Range(0, double.MaxValue)] double PricePerKg);
