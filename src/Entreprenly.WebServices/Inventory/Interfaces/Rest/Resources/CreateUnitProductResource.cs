using System.ComponentModel.DataAnnotations;

namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource received to create a unit product.
/// </summary>
public record CreateUnitProductResource(
    [Required] string Name,
    string? Description,
    string? CodeQr,
    [Range(0, double.MaxValue)] double Price,
    [Range(0, double.MaxValue)] double WeightGrams,
    string? Brand);
