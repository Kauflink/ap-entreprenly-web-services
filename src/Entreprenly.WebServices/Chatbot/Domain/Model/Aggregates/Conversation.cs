using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;

public class Conversation
{
    public Conversation()
    {
        ClientPhone = string.Empty;
        ClientName  = string.Empty;
        Status      = ConversationStatus.Active;
        StartedAt   = DateTime.UtcNow;
    }

    public Conversation(int sellerId, string clientPhone, string clientName)
    {
        SellerId    = sellerId;
        ClientPhone = clientPhone;
        ClientName  = clientName;
        Status      = ConversationStatus.Active;
        StartedAt   = DateTime.UtcNow;
    }

    public int                 Id          { get; private set; }
    public int                 SellerId    { get; private set; }
    public string              ClientPhone { get; private set; }
    public string              ClientName  { get; private set; }
    public ConversationStatus  Status      { get; private set; }
    public DateTime            StartedAt   { get; private set; }
    public DateTime?           ClosedAt    { get; private set; }

    public Conversation UpdateStatus(ConversationStatus status)
    {
        Status   = status;
        if (status is ConversationStatus.Completed or ConversationStatus.Closed)
            ClosedAt = DateTime.UtcNow;
        return this;
    }
}
