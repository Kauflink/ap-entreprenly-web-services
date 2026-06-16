using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

namespace Entreprenly.WebServices.Chatbot.Application.QueryServices;

public interface IChatMessageQueryService
{
    Task<IEnumerable<ChatMessage>> Handle(GetAllChatMessagesQuery query, CancellationToken cancellationToken);
    Task<IEnumerable<ChatMessage>> Handle(GetChatMessagesByConversationIdQuery query, CancellationToken cancellationToken);
}
