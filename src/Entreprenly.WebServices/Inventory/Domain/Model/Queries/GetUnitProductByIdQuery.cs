namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get a unit product owned by an account by its identifier.
/// </summary>
public record GetUnitProductByIdQuery(string OwnerEmail, int UnitProductId);
