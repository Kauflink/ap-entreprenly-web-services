namespace Entreprenly.WebServices.Chatbot.Domain.Services;

public interface IChatbotResponder
{
    Task<string?> GenerateReplyAsync(string incomingMessage, string clientName, CancellationToken cancellationToken);
}
