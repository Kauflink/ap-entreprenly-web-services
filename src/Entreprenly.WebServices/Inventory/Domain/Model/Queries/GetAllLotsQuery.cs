namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get the combined (unit + weight) lot view owned by an account.
/// </summary>
public record GetAllLotsQuery(string OwnerEmail);
