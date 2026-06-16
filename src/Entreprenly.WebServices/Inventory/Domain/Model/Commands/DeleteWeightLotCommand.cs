namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to delete a weight lot owned by an account.
/// </summary>
public record DeleteWeightLotCommand(string OwnerEmail, int WeightLotId);
