namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

/// <summary>
///     Command to start a new chatbot conversation with a client.
/// </summary>
public record CreateConversationCommand(int SellerId, string ClientPhone, string ClientName);
