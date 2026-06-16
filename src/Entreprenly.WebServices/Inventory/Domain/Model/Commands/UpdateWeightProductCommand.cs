namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to update an existing weight product owned by an account.
/// </summary>
public record UpdateWeightProductCommand(
    string OwnerEmail,
    int WeightProductId,
    string Name,
    string? Description,
    string? CodeQr,
    double PricePerKg);
