namespace Entreprenly.WebServices.Inventory.Domain.Model.Queries;

/// <summary>
///     Query to get a single currently raised stock alert by its run-scoped identifier.
/// </summary>
public record GetStockAlertByIdQuery(string OwnerEmail, int StockAlertId);
