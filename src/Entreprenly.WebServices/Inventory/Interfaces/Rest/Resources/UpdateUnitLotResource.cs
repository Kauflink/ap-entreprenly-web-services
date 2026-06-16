using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to update a unit lot.
/// </summary>
public record UpdateUnitLotResource(
    [Required] int ProductId,
    string? CodeQr,
    DateTimeOffset? EntryDate,
    [Range(0, int.MaxValue)] int Quantity,
    DateTimeOffset? ExpiryDate);
