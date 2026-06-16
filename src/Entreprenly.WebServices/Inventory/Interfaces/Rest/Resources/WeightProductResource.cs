namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource representing a weight product returned by the REST API.
/// </summary>
public record WeightProductResource(
    int Id,
    string Name,
    string? Description,
    string? CodeQr,
    string ProductType,
    double PricePerKg);
