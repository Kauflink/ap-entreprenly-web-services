namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource representing a unit lot returned by the REST API.
/// </summary>
public record UnitLotResource(
    int Id,
    int ProductId,
    string? CodeQr,
    DateTimeOffset EntryDate,
    string LotType,
    int Quantity,
    DateTimeOffset? ExpiryDate);
