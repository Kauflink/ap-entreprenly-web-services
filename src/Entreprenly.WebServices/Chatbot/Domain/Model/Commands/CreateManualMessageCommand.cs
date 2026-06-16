namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

public record CreateManualMessageCommand(int ConversationId, string Content, string Sender, string Type);
