namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

/// <summary>
///     Command to process an inbound payment receipt image sent by a client via WhatsApp.
/// </summary>
public record HandleInboundReceiptCommand(string FromPhone, string OwnerEmail, string Image);
