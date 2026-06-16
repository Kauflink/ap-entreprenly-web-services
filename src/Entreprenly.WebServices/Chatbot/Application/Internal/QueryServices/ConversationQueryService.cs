using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Application.Internal.QueryServices;

public class ConversationQueryService(
    IConversationRepository conversationRepository,
    IChatMessageRepository chatMessageRepository)
    : IConversationQueryService
{
    public async Task<IEnumerable<(Conversation, ChatMessage?)>> Handle(
        GetAllConversationsQuery query, CancellationToken cancellationToken)
    {
        var conversations = query.SellerId.HasValue
            ? await conversationRepository.FindAllBySellerIdAsync(query.SellerId.Value, cancellationToken)
            : await conversationRepository.ListAsync(cancellationToken);

        var result = new List<(Conversation, ChatMessage?)>();
        foreach (var conv in conversations)
        {
            var last = await chatMessageRepository.FindLastByConversationIdAsync(conv.Id, cancellationToken);
            result.Add((conv, last));
        }

        return result;
    }

    public async Task<Conversation?> Handle(GetConversationByIdQuery query, CancellationToken cancellationToken)
    {
        return await conversationRepository.FindByIdAsync(query.ConversationId, cancellationToken);
    }
}
