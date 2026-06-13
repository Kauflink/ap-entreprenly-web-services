using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Application.Internal.QueryServices;

public class ConversationQueryService(IConversationRepository conversationRepository) : IConversationQueryService
{
    public async Task<IEnumerable<Conversation>> Handle(GetAllConversationsQuery query,
        CancellationToken cancellationToken)
    {
        if (query.SellerId.HasValue)
            return await conversationRepository.FindAllBySellerIdAsync(query.SellerId.Value, cancellationToken);

        return await conversationRepository.ListAsync(cancellationToken);
    }

    public async Task<Conversation?> Handle(GetConversationByIdQuery query, CancellationToken cancellationToken)
    {
        return await conversationRepository.FindByIdAsync(query.ConversationId, cancellationToken);
    }
}
