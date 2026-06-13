using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

public record CreateChatOrderResource(
    int ConversationId,
    int SellerId,
    string ClientPhone,
    string DeliveryAddress,
    List<OrderItem> Items);
