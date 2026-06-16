namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

public record ChatMessageResource(
    int Id,
    int ConversationId,
    string Content,
    string Sender,
    string Type,
    DateTime SentAt);
