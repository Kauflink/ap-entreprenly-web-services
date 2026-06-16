using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;

public static class ChatOrderResourceFromEntityAssembler
{
    private static string ToFrontendStatus(OrderStatus s) => s switch
    {
        OrderStatus.WaitingPayment => "WAITING_PAYMENT",
        _ => s.ToString().ToUpperInvariant()
    };

    public static ChatOrderResource ToResourceFromEntity(ChatOrder order) =>
        new(order.Id, order.ConversationId, order.SellerId, order.OrderNumber,
            order.ClientPhone, order.DeliveryAddress, order.Items, order.Total,
            ToFrontendStatus(order.Status), order.HasReceipt, order.ReceiptImageUrl,
            order.RejectionCount, order.CreatedAt);
}
