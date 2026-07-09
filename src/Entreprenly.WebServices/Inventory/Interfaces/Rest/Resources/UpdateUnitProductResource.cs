using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to update a unit product.
/// </summary>
public record UpdateUnitProductResource(
    [Required] string Name,
    string? Description,
    [property: JsonPropertyName("codeQR")] string? CodeQr,
    [Range(0, double.MaxValue)] double Price,
    [Range(0, double.MaxValue)] double WeightGrams,
    string? Brand);
