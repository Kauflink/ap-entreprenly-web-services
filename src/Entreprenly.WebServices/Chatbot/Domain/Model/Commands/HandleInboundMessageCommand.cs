namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

/// <summary>
///     Command to process an inbound text message received from a client via WhatsApp.
/// </summary>
public record HandleInboundMessageCommand(string FromPhone, string ClientName, string Content, string OwnerEmail);
