using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Application.Internal.QueryServices;

public class ChatOrderQueryService(IChatOrderRepository chatOrderRepository) : IChatOrderQueryService
{
    public async Task<IEnumerable<ChatOrder>> Handle(GetAllChatOrdersQuery query, CancellationToken cancellationToken)
    {
        if (query.SellerId.HasValue)
            return await chatOrderRepository.FindAllBySellerIdAsync(query.SellerId.Value, cancellationToken);

        return await chatOrderRepository.ListAsync(cancellationToken);
    }

    public async Task<ChatOrder?> Handle(GetChatOrderByIdQuery query, CancellationToken cancellationToken)
    {
        return await chatOrderRepository.FindByIdAsync(query.ChatOrderId, cancellationToken);
    }
}
