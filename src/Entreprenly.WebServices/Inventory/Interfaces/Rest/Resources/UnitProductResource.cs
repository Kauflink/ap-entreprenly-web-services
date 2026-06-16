namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource representing a unit product returned by the REST API.
/// </summary>
public record UnitProductResource(
    int Id,
    string Name,
    string? Description,
    string? CodeQr,
    string ProductType,
    double Price,
    double WeightGrams,
    string? Brand);
