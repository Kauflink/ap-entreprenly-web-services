namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to create a new unit product for an owner account.
/// </summary>
public record CreateUnitProductCommand(
    string OwnerEmail,
    string Name,
    string? Description,
    string? CodeQr,
    double Price,
    double WeightGrams,
    string? Brand);
