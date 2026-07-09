namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to update an existing unit lot owned by an account.
/// </summary>
public record UpdateUnitLotCommand(
    string OwnerEmail,
    int UnitLotId,
    int ProductId,
    string? CodeQr,
    DateTimeOffset? EntryDate,
    int Quantity,
    DateTimeOffset? ExpiryDate);
