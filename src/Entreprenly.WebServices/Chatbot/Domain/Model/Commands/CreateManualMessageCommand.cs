namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

/// <summary>
///     Command to append a manually written message to an existing conversation.
/// </summary>
public record CreateManualMessageCommand(int ConversationId, string Content, string Sender, string Type);
