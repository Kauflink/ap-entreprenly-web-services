using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class StubCatalogProductRepository : ICatalogProductRepository
{
    public Task<IEnumerable<CatalogProduct>> FindByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken)
        => Task.FromResult(Enumerable.Empty<CatalogProduct>());
}
