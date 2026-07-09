namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get every unit product owned by an account.
/// </summary>
public record GetAllUnitProductsQuery(string OwnerEmail);
