using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Repositories;

public interface ICatalogProductRepository
{
    Task<IEnumerable<CatalogProduct>> FindByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken);
}
