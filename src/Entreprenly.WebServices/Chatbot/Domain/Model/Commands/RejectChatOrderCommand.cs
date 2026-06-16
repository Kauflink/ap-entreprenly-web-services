namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

public record RejectChatOrderCommand(int ChatOrderId, string Reason);
