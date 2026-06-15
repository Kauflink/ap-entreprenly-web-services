namespace Entreprenly.WebServices.Sales.Domain.Model.Queries;

/// <summary>
///     Query to retrieve every cash register owned by the authenticated account.
/// </summary>
public record GetAllCashRegistersQuery(string OwnerEmail);
