using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

/// <summary>
///     Command to register a new chat order captured from a chatbot conversation.
/// </summary>
public record CreateChatOrderCommand(
    int ConversationId,
    int SellerId,
    string OwnerEmail,
    string ClientPhone,
    string DeliveryAddress,
    List<OrderItem> Items);
