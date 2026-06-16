using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Inventory.Domain.Model.Entities;

/// <summary>
///     Stock alert read model. A stock alert is a derived (computed) view raised from the current
///     state of inventory products and their lots; it is never persisted. It signals low stock,
///     out-of-stock, soon-to-expire and expired situations so the storefront can react.
/// </summary>
public class StockAlert(
    int id,
    int? lotId,
    int productId,
    ProductType? productType,
    string productName,
    AlertType alertType,
    AlertSeverity severity,
    string message,
    DateTimeOffset createdAt)
{
    public int Id { get; } = id;
    public int? LotId { get; } = lotId;
    public int ProductId { get; } = productId;
    public ProductType? ProductType { get; } = productType;
    public string ProductName { get; } = productName;
    public AlertType AlertType { get; } = alertType;
    public AlertSeverity Severity { get; } = severity;
    public string Message { get; } = message;
    public DateTimeOffset CreatedAt { get; } = createdAt;

    /// <summary>
    ///     Returns a copy of this alert with the given identifier assigned.
    /// </summary>
    public StockAlert WithId(int newId)
    {
        return new StockAlert(newId, LotId, ProductId, ProductType, ProductName, AlertType, Severity, Message,
            CreatedAt);
    }
}
