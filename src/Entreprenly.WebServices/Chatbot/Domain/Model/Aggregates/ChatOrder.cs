using System.Text.Json;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;

/// <summary>
///     Aggregate root representing an order captured from a chatbot conversation, tracking its lifecycle from
///     draft through payment confirmation or cancellation.
/// </summary>
public class ChatOrder
{
    public ChatOrder()
    {
        OrderNumber     = string.Empty;
        OwnerEmail      = string.Empty;
        ClientPhone     = string.Empty;
        DeliveryAddress = string.Empty;
        ItemsJson       = "[]";
        Status          = OrderStatus.WaitingPayment;
        CreatedAt       = DateTime.UtcNow;
    }

    public ChatOrder(int conversationId, int sellerId, string ownerEmail, string clientPhone, List<OrderItem> items)
    {
        ConversationId  = conversationId;
        SellerId        = sellerId;
        OwnerEmail      = ownerEmail;
        ClientPhone     = clientPhone;
        DeliveryAddress = string.Empty;
        ItemsJson       = JsonSerializer.Serialize(items);
        Total           = items.Sum(i => i.Subtotal);
        Status          = OrderStatus.Pending;
        HasReceipt      = false;
        RejectionCount  = 0;
        CreatedAt       = DateTime.UtcNow;
        OrderNumber     = GenerateOrderNumber();
    }

    public int         Id              { get; private set; }
    public int         ConversationId  { get; private set; }
    public int         SellerId        { get; private set; }
    public string      OwnerEmail      { get; private set; }
    public string      OrderNumber     { get; private set; }
    public string      ClientPhone     { get; private set; }
    public string      DeliveryAddress { get; set; }
    public string      ItemsJson       { get; private set; }
    public decimal     Total           { get; private set; }
    public OrderStatus Status          { get; private set; }
    public bool        HasReceipt      { get; private set; }
    public string?     ReceiptImageUrl { get; private set; }
    public int         RejectionCount  { get; private set; }
    public DateTime    CreatedAt       { get; private set; }

    public List<OrderItem> Items =>
        JsonSerializer.Deserialize<List<OrderItem>>(ItemsJson) ?? [];

    /// <summary>
    ///     Sets the delivery address and advances the order to the waiting-payment state.
    /// </summary>
    public ChatOrder ConfirmDelivery(string deliveryAddress)
    {
        DeliveryAddress = deliveryAddress;
        Status          = OrderStatus.WaitingPayment;
        return this;
    }

    /// <summary>
    ///     Attaches a payment receipt image URL to the order.
    /// </summary>
    public ChatOrder AttachReceipt(string receiptImageUrl)
    {
        HasReceipt      = true;
        ReceiptImageUrl = receiptImageUrl;
        return this;
    }

    /// <summary>
    ///     Confirms the order after successful payment validation.
    /// </summary>
    public ChatOrder Confirm()
    {
        Status = OrderStatus.Confirmed;
        return this;
    }

    /// <summary>
    ///     Rejects the payment receipt; blocks the order after two consecutive rejections.
    /// </summary>
    public ChatOrder Reject()
    {
        RejectionCount++;
        HasReceipt = false;
        if (RejectionCount >= 2) Status = OrderStatus.Blocked;
        return this;
    }

    /// <summary>
    ///     Cancels the order.
    /// </summary>
    public ChatOrder Cancel()
    {
        Status = OrderStatus.Cancelled;
        return this;
    }

    private static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
}
