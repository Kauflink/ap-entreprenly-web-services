using System.Text.Json;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;

public class ChatOrder
{
    public ChatOrder()
    {
        OrderNumber     = string.Empty;
        ClientPhone     = string.Empty;
        DeliveryAddress = string.Empty;
        ItemsJson       = "[]";
        Status          = OrderStatus.WaitingPayment;
        CreatedAt       = DateTime.UtcNow;
    }

    public ChatOrder(int conversationId, int sellerId, string clientPhone, List<OrderItem> items)
    {
        ConversationId  = conversationId;
        SellerId        = sellerId;
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

    public ChatOrder(int conversationId, int sellerId, string clientPhone, string deliveryAddress,
        List<OrderItem> items)
    {
        ConversationId  = conversationId;
        SellerId        = sellerId;
        ClientPhone     = clientPhone;
        DeliveryAddress = deliveryAddress;
        ItemsJson       = JsonSerializer.Serialize(items);
        Total           = items.Sum(i => i.Subtotal);
        Status          = OrderStatus.WaitingPayment;
        HasReceipt      = false;
        RejectionCount  = 0;
        CreatedAt       = DateTime.UtcNow;
        OrderNumber     = GenerateOrderNumber();
    }

    public int         Id              { get; private set; }
    public int         ConversationId  { get; private set; }
    public int         SellerId        { get; private set; }
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

    public ChatOrder ConfirmDelivery(string deliveryAddress)
    {
        DeliveryAddress = deliveryAddress;
        Status          = OrderStatus.WaitingPayment;
        return this;
    }

    public ChatOrder AttachReceipt(string receiptImageUrl)
    {
        HasReceipt      = true;
        ReceiptImageUrl = receiptImageUrl;
        return this;
    }

    public ChatOrder Confirm()
    {
        Status = OrderStatus.Confirmed;
        return this;
    }

    public ChatOrder Reject()
    {
        RejectionCount++;
        HasReceipt = false;
        if (RejectionCount >= 2) Status = OrderStatus.Blocked;
        return this;
    }

    public ChatOrder Cancel()
    {
        Status = OrderStatus.Cancelled;
        return this;
    }

    private static string GenerateOrderNumber() =>
        $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
}
