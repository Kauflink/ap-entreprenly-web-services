using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Domain.Repositories;

public interface IChatOrderRepository : IBaseRepository<ChatOrder>
{
    Task<ChatOrder?> FindByConversationIdAsync(int conversationId, CancellationToken cancellationToken);
    Task<IEnumerable<ChatOrder>> FindAllBySellerIdAsync(int sellerId, CancellationToken cancellationToken);
}
