namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to update an existing weight lot owned by an account.
/// </summary>
public record UpdateWeightLotCommand(
    string OwnerEmail,
    int WeightLotId,
    int ProductId,
    string? CodeQr,
    DateTimeOffset? EntryDate,
    double QuantityKg);
