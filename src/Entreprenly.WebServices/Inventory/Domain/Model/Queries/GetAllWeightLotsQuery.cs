namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get every weight lot owned by an account.
/// </summary>
public record GetAllWeightLotsQuery(string OwnerEmail);
