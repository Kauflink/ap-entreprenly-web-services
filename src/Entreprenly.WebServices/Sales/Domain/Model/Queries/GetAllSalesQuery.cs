namespace Entreprenly.WebServices.Sales.Domain.Model.Queries;

/// <summary>
///     Query to retrieve every sale owned by the authenticated account.
/// </summary>
public record GetAllSalesQuery(string OwnerEmail);
