using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

namespace Entreprenly.WebServices.Chatbot.Application.QueryServices;

public interface IConversationQueryService
{
    Task<IEnumerable<Conversation>> Handle(GetAllConversationsQuery query, CancellationToken cancellationToken);
    Task<Conversation?> Handle(GetConversationByIdQuery query, CancellationToken cancellationToken);
}
