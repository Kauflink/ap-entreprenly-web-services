using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to update a unit product.
/// </summary>
public record UpdateUnitProductResource(
    [Required] string Name,
    string? Description,
    string? CodeQr,
    [Range(0, double.MaxValue)] double Price,
    [Range(0, double.MaxValue)] double WeightGrams,
    string? Brand);
