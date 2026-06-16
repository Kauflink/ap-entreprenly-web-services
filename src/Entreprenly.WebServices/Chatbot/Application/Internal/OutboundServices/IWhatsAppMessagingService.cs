namespace Entreprenly.WebServices.Chatbot.Application.Internal.OutboundServices;

public interface IWhatsAppMessagingService
{
    Task SendMessageAsync(string ownerEmail, string phone, string content, CancellationToken cancellationToken);
}
