namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to create a new weight lot for an owner account.
/// </summary>
public record CreateWeightLotCommand(
    string OwnerEmail,
    int ProductId,
    string? CodeQr,
    DateTimeOffset? EntryDate,
    double QuantityKg);
