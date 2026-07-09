namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

/// <summary>
///     Command to confirm a chat order after successful payment validation.
/// </summary>
public record ConfirmChatOrderCommand(int ChatOrderId);
