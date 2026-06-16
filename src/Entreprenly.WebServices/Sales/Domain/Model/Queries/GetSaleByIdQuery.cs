namespace Entreprenly.WebServices.Sales.Domain.Model.Queries;

/// <summary>
///     Query to retrieve a single sale by its identifier, scoped to the authenticated account.
/// </summary>
public record GetSaleByIdQuery(string OwnerEmail, int SaleId);
