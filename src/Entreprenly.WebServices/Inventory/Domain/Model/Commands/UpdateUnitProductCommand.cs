namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to update an existing unit product owned by an account.
/// </summary>
public record UpdateUnitProductCommand(
    string OwnerEmail,
    int UnitProductId,
    string Name,
    string? Description,
    string? CodeQr,
    double Price,
    double WeightGrams,
    string? Brand);
