namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get a weight product owned by an account by its identifier.
/// </summary>
public record GetWeightProductByIdQuery(string OwnerEmail, int WeightProductId);
