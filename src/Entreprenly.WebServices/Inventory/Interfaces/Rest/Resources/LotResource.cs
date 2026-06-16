namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource representing a lot in the combined (unit + weight) read view.
/// </summary>
public record LotResource(
    int Id,
    int ProductId,
    string? CodeQr,
    DateTimeOffset EntryDate,
    string LotType);
