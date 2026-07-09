using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to create a unit product.
/// </summary>
public record CreateUnitProductResource(
    [Required] string Name,
    string? Description,
    [property: JsonPropertyName("codeQR")] string? CodeQr,
    [Range(0, double.MaxValue)] double Price,
    [Range(0, double.MaxValue)] double WeightGrams,
    string? Brand);
