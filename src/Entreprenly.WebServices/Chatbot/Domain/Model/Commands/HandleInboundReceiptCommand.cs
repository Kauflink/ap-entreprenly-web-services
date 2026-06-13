namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

public record HandleInboundReceiptCommand(string FromPhone, string OwnerEmail, string Image);
