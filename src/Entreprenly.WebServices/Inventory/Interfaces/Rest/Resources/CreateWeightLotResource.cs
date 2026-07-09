using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to create a weight lot.
/// </summary>
public record CreateWeightLotResource(
    [Required] int ProductId,
    [property: JsonPropertyName("codeQR")] string? CodeQr,
    DateTimeOffset? EntryDate,
    [Range(0, double.MaxValue)] double QuantityKg);
