namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

public record ConversationResource(
    int Id,
    int SellerId,
    string ClientPhone,
    string ClientName,
    string Status,
    DateTime StartedAt,
    DateTime? ClosedAt,
    string? LastMessage,
    DateTime? LastMessageAt);
