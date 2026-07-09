using Entreprenly.WebServices.Inventory.Interfaces.Acl;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;

/// <summary>
///     Translates an Inventory <see cref="CatalogItem" /> (obtained through the Inventory ACL) into a
///     <see cref="SalesProductResource" /> for the point-of-sale view.
/// </summary>
public static class SalesProductResourceFromCatalogItemAssembler
{
    public static SalesProductResource ToResourceFromCatalogItem(CatalogItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return new SalesProductResource(item.Name, item.Price, item.ByWeight, item.Stock);
    }
}
