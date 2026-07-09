using System.Text.Json.Serialization;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource representing a weight product returned by the REST API.
/// </summary>
public record WeightProductResource(
    int Id,
    string Name,
    string? Description,
    [property: JsonPropertyName("codeQR")] string? CodeQr,
    string ProductType,
    double PricePerKg);
