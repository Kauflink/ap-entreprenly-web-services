namespace Entreprenly.WebServices.Sales.Domain.Model.Queries;

/// <summary>
///     Query to retrieve the cash register of a business day, scoped to the authenticated account.
/// </summary>
public record GetCashRegisterByDateQuery(string OwnerEmail, DateOnly Date);
