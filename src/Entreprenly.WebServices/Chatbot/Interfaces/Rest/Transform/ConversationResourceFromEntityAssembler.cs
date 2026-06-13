using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;

public static class ConversationResourceFromEntityAssembler
{
    public static ConversationResource ToResourceFromEntity(Conversation conversation) =>
        new(conversation.Id, conversation.SellerId, conversation.ClientPhone,
            conversation.ClientName, conversation.Status.ToString(),
            conversation.StartedAt, conversation.ClosedAt);
}
