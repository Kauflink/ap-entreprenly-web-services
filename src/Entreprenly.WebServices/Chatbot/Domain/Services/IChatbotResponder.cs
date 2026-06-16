namespace Entreprenly.WebServices.Chatbot.Domain.Services;

public interface IChatbotResponder
{
    Task<string?> GenerateReplyAsync(string incomingMessage, int sellerId, CancellationToken cancellationToken);
}
