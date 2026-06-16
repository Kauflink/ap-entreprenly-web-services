namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

public record CreateConversationCommand(int SellerId, string ClientPhone, string ClientName);
