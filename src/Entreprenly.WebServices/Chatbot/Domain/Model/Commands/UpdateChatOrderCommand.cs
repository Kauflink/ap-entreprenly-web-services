using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Commands;

public record UpdateChatOrderCommand(int ChatOrderId, OrderStatus Status, bool HasReceipt, int RejectionCount);
