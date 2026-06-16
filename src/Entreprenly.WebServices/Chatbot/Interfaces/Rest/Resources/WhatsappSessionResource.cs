namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

public record WhatsappSessionResource(
    int Id,
    int SellerId,
    string OwnerEmail,
    string BusinessName,
    string Status,
    string? Phone,
    DateTime? ConnectedAt);
