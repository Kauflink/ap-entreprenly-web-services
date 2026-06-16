namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource representing a weight lot returned by the REST API.
/// </summary>
public record WeightLotResource(
    int Id,
    int ProductId,
    string? CodeQr,
    DateTimeOffset EntryDate,
    string LotType,
    double QuantityKg);
