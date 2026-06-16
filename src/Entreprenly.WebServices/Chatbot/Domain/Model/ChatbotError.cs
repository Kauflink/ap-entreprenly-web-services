namespace Entreprenly.WebServices.Chatbot.Domain.Model;

public enum ChatbotError
{
    SessionNotFound,
    ConversationNotFound,
    OrderNotFound,
    DatabaseError,
    OperationCancelled,
    InternalServerError
}
