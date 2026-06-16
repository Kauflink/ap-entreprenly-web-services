namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

public record HandleInboundMessageCommand(string FromPhone, string ClientName, string Content, string OwnerEmail);
