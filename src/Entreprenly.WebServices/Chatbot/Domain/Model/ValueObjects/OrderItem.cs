namespace Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;

public record OrderItem(
    int ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity
)
{
    public decimal Subtotal => UnitPrice * Quantity;
}
