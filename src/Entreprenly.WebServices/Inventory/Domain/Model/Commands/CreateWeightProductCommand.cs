namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to create a new weight product for an owner account.
/// </summary>
public record CreateWeightProductCommand(
    string OwnerEmail,
    string Name,
    string? Description,
    string? CodeQr,
    double PricePerKg);
