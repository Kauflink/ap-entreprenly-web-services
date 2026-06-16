namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get a weight lot owned by an account by its identifier.
/// </summary>
public record GetWeightLotByIdQuery(string OwnerEmail, int WeightLotId);
