using Entreprenly.WebServices.Shared.Domain.Model;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Errors;

public static class ChatbotErrors
{
    public static readonly Error SessionNotFound =
        new("Chatbot.SessionNotFound", "WhatsApp session not found.");

    public static readonly Error ConversationNotFound =
        new("Chatbot.ConversationNotFound", "Conversation not found.");

    public static readonly Error OrderNotFound =
        new("Chatbot.OrderNotFound", "Order not found.");

    public static readonly Error DatabaseError =
        new("Chatbot.DatabaseError", "A database error occurred.");

    public static readonly Error OperationCancelled =
        new("Chatbot.OperationCancelled", "The operation was cancelled.");

    public static readonly Error InternalServerError =
        new("Chatbot.InternalServerError", "An unexpected internal error occurred.");
}
