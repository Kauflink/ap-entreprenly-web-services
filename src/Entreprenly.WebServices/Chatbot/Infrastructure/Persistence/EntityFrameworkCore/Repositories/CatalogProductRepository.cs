using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Inventory.Interfaces.Acl;

namespace Entreprenly.WebServices.Chatbot.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class CatalogProductRepository(IInventoryContextFacade inventory) : ICatalogProductRepository
{
    public async Task<IEnumerable<CatalogProduct>> FindByOwnerEmailAsync(
        string ownerEmail, CancellationToken cancellationToken)
    {
        var items = (await inventory.FetchCatalogByOwnerAsync(ownerEmail, cancellationToken)).ToList();

        return items.Select((item, i) => new CatalogProduct(
            Id:           i + 1,
            Name:         item.Name,
            Price:        (decimal)item.Price,
            SoldByWeight: item.ByWeight,
            Stock:        (decimal)item.Stock));
    }
}
