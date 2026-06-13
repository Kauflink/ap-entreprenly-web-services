using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;

public class ChatMessage
{
    public ChatMessage()
    {
        Content = string.Empty;
        SentAt  = DateTime.UtcNow;
    }

    public ChatMessage(int conversationId, string content, MessageSender sender, MessageType type)
    {
        ConversationId = conversationId;
        Content        = content;
        Sender         = sender;
        Type           = type;
        SentAt         = DateTime.UtcNow;
    }

    public int           Id             { get; private set; }
    public int           ConversationId { get; private set; }
    public string        Content        { get; private set; }
    public MessageSender Sender         { get; private set; }
    public MessageType   Type           { get; private set; }
    public DateTime      SentAt         { get; private set; }
}
