namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to compute every currently raised stock alert for an account.
/// </summary>
public record GetAllStockAlertsQuery(string OwnerEmail);
