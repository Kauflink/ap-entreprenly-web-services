using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

/// <summary>
///     Command to update the status of an existing chatbot conversation.
/// </summary>
public record UpdateConversationCommand(int ConversationId, ConversationStatus Status);
