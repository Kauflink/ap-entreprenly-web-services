namespace Entreprenly.WebServices.Sales.Domain.Model.Queries;

/// <summary>
///     Query to retrieve the sales registered by the authenticated account on a given business day.
/// </summary>
/// <param name="OwnerEmail">The authenticated account whose sales are requested.</param>
/// <param name="Date">The business day (local date) used to filter the sales.</param>
public record GetSalesByDateQuery(string OwnerEmail, DateOnly Date);
