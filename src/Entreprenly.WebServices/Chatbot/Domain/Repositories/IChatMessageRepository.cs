using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Domain.Repositories;

public interface IChatMessageRepository : IBaseRepository<ChatMessage>
{
    Task<IEnumerable<ChatMessage>> FindAllByConversationIdAsync(int conversationId,
        CancellationToken cancellationToken);

    Task<ChatMessage?> FindLastByConversationIdAsync(int conversationId,
        CancellationToken cancellationToken);

    Task<IEnumerable<ChatMessage>> FindAllBySellerIdAsync(int sellerId, CancellationToken cancellationToken);
}
