namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

public record CreateChatMessageResource(int ConversationId, string Content, string Sender, string Type);
