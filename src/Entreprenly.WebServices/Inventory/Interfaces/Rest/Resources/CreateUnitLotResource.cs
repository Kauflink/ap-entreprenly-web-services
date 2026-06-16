using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to create a unit lot.
/// </summary>
public record CreateUnitLotResource(
    [Required] int ProductId,
    string? CodeQr,
    DateTimeOffset? EntryDate,
    [Range(0, int.MaxValue)] int Quantity,
    DateTimeOffset? ExpiryDate);
