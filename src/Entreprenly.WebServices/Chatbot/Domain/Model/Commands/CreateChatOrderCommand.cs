using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

public record CreateChatOrderCommand(
    int ConversationId,
    int SellerId,
    string ClientPhone,
    string DeliveryAddress,
    List<OrderItem> Items);
