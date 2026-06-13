using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Application.Internal.QueryServices;

public class ChatMessageQueryService(IChatMessageRepository chatMessageRepository) : IChatMessageQueryService
{
    public async Task<IEnumerable<ChatMessage>> Handle(GetAllChatMessagesQuery query,
        CancellationToken cancellationToken)
    {
        return await chatMessageRepository.ListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ChatMessage>> Handle(GetChatMessagesByConversationIdQuery query,
        CancellationToken cancellationToken)
    {
        return await chatMessageRepository.FindAllByConversationIdAsync(query.ConversationId, cancellationToken);
    }
}
