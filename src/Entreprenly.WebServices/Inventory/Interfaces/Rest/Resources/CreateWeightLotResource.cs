using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to create a weight lot.
/// </summary>
public record CreateWeightLotResource(
    [Required] int ProductId,
    string? CodeQr,
    DateTimeOffset? EntryDate,
    [Range(0, double.MaxValue)] double QuantityKg);
