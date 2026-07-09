namespace Entreprenly.WebServices.Sales.Interfaces.Acl;

/// <summary>
///     A line of a sale originated in the chatbot, used by other bounded contexts through the
///     <see cref="ISalesContextFacade" />.
/// </summary>
/// <param name="ProductName">The product display name agreed in the chat.</param>
/// <param name="Quantity">The number of units sold.</param>
/// <param name="UnitPrice">The price per unit at sale time.</param>
public record ChatSaleLine(string ProductName, int Quantity, double UnitPrice);
