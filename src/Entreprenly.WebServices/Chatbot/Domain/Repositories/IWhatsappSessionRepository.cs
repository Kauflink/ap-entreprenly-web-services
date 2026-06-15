using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Domain.Repositories;

public interface IWhatsappSessionRepository : IBaseRepository<WhatsappSession>
{
    Task<WhatsappSession?> FindByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken);
    Task<WhatsappSession?> FindBySellerIdAsync(int sellerId, CancellationToken cancellationToken);
    Task<IEnumerable<WhatsappSession>> FindAllBySellerIdAsync(int sellerId, CancellationToken cancellationToken);
    Task<bool> ExistsByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken);
}
