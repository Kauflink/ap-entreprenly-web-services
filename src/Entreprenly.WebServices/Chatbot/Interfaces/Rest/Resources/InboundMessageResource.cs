namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

public record InboundMessageResource(string FromPhone, string ClientName, string Content, string OwnerEmail);
