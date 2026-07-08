using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to create a weight product.
/// </summary>
public record CreateWeightProductResource(
    [Required] string Name,
    string? Description,
    [property: JsonPropertyName("codeQR")] string? CodeQr,
    [Range(0, double.MaxValue)] double PricePerKg);
