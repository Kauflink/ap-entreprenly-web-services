using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing an active WhatsApp Business connection for a seller, managed by the Node.js bridge.
/// </summary>
public class WhatsappSession
{
    public WhatsappSession()
    {
        OwnerEmail    = string.Empty;
        BusinessName  = string.Empty;
        Status        = SessionStatus.Disconnected;
    }

    public WhatsappSession(int sellerId, string ownerEmail, string businessName)
    {
        SellerId     = sellerId;
        OwnerEmail   = ownerEmail;
        BusinessName = businessName;
        Status       = SessionStatus.Disconnected;
    }

    public int            Id           { get; private set; }
    public int            SellerId     { get; private set; }
    public string         OwnerEmail   { get; private set; }
    public string         BusinessName { get; private set; }
    public SessionStatus  Status       { get; private set; }
    public string?        Phone        { get; private set; }
    public DateTime?      ConnectedAt  { get; private set; }

    /// <summary>
    ///     Marks the session as connected and records the WhatsApp phone number and connection time.
    /// </summary>
    public WhatsappSession ReportConnected(string phone)
    {
        Status      = SessionStatus.Connected;
        Phone       = phone;
        ConnectedAt = DateTime.UtcNow;
        return this;
    }

    /// <summary>
    ///     Marks the session as disconnected and clears the associated phone number.
    /// </summary>
    public WhatsappSession ReportDisconnected()
    {
        Status      = SessionStatus.Disconnected;
        Phone       = null;
        ConnectedAt = null;
        return this;
    }
}
