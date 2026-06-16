using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

namespace Entreprenly.WebServices.Chatbot.Application.QueryServices;

public interface IChatOrderQueryService
{
    Task<IEnumerable<ChatOrder>> Handle(GetAllChatOrdersQuery query, CancellationToken cancellationToken);
    Task<ChatOrder?> Handle(GetChatOrderByIdQuery query, CancellationToken cancellationToken);
}
