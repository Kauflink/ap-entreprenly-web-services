using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class StubCatalogProductRepository : ICatalogProductRepository
{
    public Task<IEnumerable<CatalogProduct>> FindBySellerIdAsync(int sellerId, CancellationToken cancellationToken)
        => Task.FromResult(Enumerable.Empty<CatalogProduct>());
}
