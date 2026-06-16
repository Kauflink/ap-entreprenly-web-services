namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to create a new unit lot for an owner account.
/// </summary>
public record CreateUnitLotCommand(
    string OwnerEmail,
    int ProductId,
    string? CodeQr,
    DateTimeOffset? EntryDate,
    int Quantity,
    DateTimeOffset? ExpiryDate);
