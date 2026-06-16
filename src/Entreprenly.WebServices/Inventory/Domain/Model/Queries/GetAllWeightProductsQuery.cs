namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get every weight product owned by an account.
/// </summary>
public record GetAllWeightProductsQuery(string OwnerEmail);
