using Entreprenly.WebServices.Shared.Domain.Model;

namespace Entreprenly.WebServices.Sales.Domain.Model.Errors;

/// <summary>
///     Catalog of Sales domain errors, exposed as reusable <see cref="Error" /> instances.
/// </summary>
public static class SalesErrors
{
    public static readonly Error OwnerRequired =
        new("Sales.OwnerRequired", "An authenticated owner is required.");

    public static readonly Error SellerRequired =
        new("Sales.SellerRequired", "A seller is required to register a sale.");

    public static readonly Error SaleItemsRequired =
        new("Sales.SaleItemsRequired", "A sale must contain at least one item.");

    public static readonly Error SaleNotFound =
        new("Sales.SaleNotFound", "Sale not found.");
}
