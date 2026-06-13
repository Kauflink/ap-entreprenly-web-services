using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

public record ChatOrderResource(
    int Id,
    int ConversationId,
    int SellerId,
    string OrderNumber,
    string ClientPhone,
    string DeliveryAddress,
    IEnumerable<OrderItem> Items,
    decimal Total,
    string Status,
    bool HasReceipt,
    string? ReceiptImageUrl,
    int RejectionCount,
    DateTime CreatedAt);
