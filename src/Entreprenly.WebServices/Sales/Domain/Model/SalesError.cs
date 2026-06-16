namespace Entreprenly.WebServices.Sales.Domain.Model;

public enum SalesError
{
    None,
    OwnerRequired,
    SellerRequired,
    SaleItemsRequired,
    SaleNotFound,
    BusinessDayRequired,
    CashRegisterAlreadyExists,
    CashRegisterNotFound,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
