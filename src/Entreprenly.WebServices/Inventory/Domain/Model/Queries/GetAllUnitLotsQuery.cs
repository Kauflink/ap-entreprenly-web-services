namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get every unit lot owned by an account.
/// </summary>
public record GetAllUnitLotsQuery(string OwnerEmail);
