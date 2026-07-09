namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

/// <summary>
///     Command to reject the payment receipt attached to a chat order.
/// </summary>
public record RejectChatOrderCommand(int ChatOrderId, string Reason);
