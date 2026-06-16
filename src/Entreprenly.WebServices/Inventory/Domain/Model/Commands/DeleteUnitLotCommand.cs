namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to delete a unit lot owned by an account.
/// </summary>
public record DeleteUnitLotCommand(string OwnerEmail, int UnitLotId);
