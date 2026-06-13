using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;

public static class ChatOrderResourceFromEntityAssembler
{
    public static ChatOrderResource ToResourceFromEntity(ChatOrder order) =>
        new(order.Id, order.ConversationId, order.SellerId, order.OrderNumber,
            order.ClientPhone, order.DeliveryAddress, order.Items, order.Total,
            order.Status.ToString(), order.HasReceipt, order.ReceiptImageUrl,
            order.RejectionCount, order.CreatedAt);
}
