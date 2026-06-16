using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Domain.Repositories;

public interface IConversationRepository : IBaseRepository<Conversation>
{
    Task<Conversation?> FindByClientPhoneAndSellerIdAsync(string clientPhone, int sellerId,
        CancellationToken cancellationToken);

    Task<IEnumerable<Conversation>> FindAllBySellerIdAsync(int sellerId, CancellationToken cancellationToken);
}
