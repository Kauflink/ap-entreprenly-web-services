using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;

public static class ConversationResourceFromEntityAssembler
{
    private static string ToFrontendStatus(ConversationStatus s) => s switch
    {
        ConversationStatus.WaitingPayment => "WAITING_PAYMENT",
        _ => s.ToString().ToUpperInvariant()
    };

    public static ConversationResource ToResourceFromEntity(
        Conversation conversation, string? lastMessage = null, DateTime? lastMessageAt = null) =>
        new(conversation.Id, conversation.SellerId, conversation.ClientPhone,
            conversation.ClientName, ToFrontendStatus(conversation.Status),
            conversation.StartedAt, conversation.ClosedAt, lastMessage, lastMessageAt);
}
