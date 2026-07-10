namespace Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

/// <summary>
///     Query to retrieve all messages belonging to a specific conversation.
/// </summary>
public record GetChatMessagesByConversationIdQuery(int ConversationId);
