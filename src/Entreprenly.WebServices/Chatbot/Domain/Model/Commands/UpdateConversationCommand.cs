using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

public record UpdateConversationCommand(int ConversationId, ConversationStatus Status);
