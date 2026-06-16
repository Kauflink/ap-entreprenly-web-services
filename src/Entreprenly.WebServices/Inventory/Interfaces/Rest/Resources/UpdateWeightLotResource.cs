using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to update a weight lot.
/// </summary>
public record UpdateWeightLotResource(
    [Required] int ProductId,
    string? CodeQr,
    DateTimeOffset? EntryDate,
    [Range(0, double.MaxValue)] double QuantityKg);
