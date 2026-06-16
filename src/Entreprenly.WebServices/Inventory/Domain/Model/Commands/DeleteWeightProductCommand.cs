namespace Entreprenly.WebServices.Inventory.Domain.Model.Commands;

/// <summary>
///     Command to delete a weight product owned by an account.
/// </summary>
public record DeleteWeightProductCommand(string OwnerEmail, int WeightProductId);
