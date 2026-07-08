using System.ComponentModel.DataAnnotations;

using System.Text.Json.Serialization;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to update a unit lot.
/// </summary>
public record UpdateUnitLotResource(
    [Required] int ProductId,
    [property: JsonPropertyName("codeQR")] string? CodeQr,
    DateTimeOffset? EntryDate,
    [Range(0, int.MaxValue)] int Quantity,
    DateTimeOffset? ExpiryDate);
