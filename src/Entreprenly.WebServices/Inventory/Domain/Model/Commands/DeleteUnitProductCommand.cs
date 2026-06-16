namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to delete a unit product owned by an account.
/// </summary>
public record DeleteUnitProductCommand(string OwnerEmail, int UnitProductId);
