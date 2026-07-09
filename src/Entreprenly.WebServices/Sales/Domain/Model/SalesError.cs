namespace Entreprenly.WebServices.Sales.Domain.Model;

/// <summary>
///     Error codes a sale command or query can fail with, mapped to HTTP status codes at the API
///     boundary.
/// </summary>
public enum SalesError
{
    None,
    OwnerRequired,
    SellerRequired,
    SaleItemsRequired,
    SaleNotFound,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
