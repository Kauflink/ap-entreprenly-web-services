namespace Entreprenly.WebServices.Sales.Domain.Model;

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
