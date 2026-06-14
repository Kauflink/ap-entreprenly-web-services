using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;

public static class ChatMessageResourceFromEntityAssembler
{
    public static ChatMessageResource ToResourceFromEntity(ChatMessage message) =>
        new(message.Id, message.ConversationId, message.Content,
            message.Sender.ToString().ToLowerInvariant(),
            message.Type.ToString().ToLowerInvariant(), message.SentAt);
}
