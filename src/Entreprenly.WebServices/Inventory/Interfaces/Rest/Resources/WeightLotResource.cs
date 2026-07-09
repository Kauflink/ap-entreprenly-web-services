using System.Text.Json.Serialization;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource representing a weight lot returned by the REST API.
/// </summary>
public record WeightLotResource(
    int Id,
    int ProductId,
    [property: JsonPropertyName("codeQR")] string? CodeQr,
    DateTimeOffset EntryDate,
    string LotType,
    double QuantityKg);
