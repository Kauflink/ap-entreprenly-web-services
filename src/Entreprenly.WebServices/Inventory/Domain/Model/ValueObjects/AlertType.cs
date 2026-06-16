namespace Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Kind of stock alert raised for a product or lot. Each type carries the lowercase wire
///     value exposed to clients and a priority used to order alerts from most to least urgent.
/// </summary>
public enum AlertType
{
    Expired,
    OutOfStock,
    ExpiringSoon,
    LowStock
}

public static class AlertTypeExtensions
{
    /// <summary>
    ///     Returns the lowercase wire value of this alert type.
    /// </summary>
    public static string ToValue(this AlertType alertType)
    {
        return alertType switch
        {
            AlertType.Expired => "expired",
            AlertType.OutOfStock => "out_of_stock",
            AlertType.ExpiringSoon => "expiring_soon",
            AlertType.LowStock => "low_stock",
            _ => alertType.ToString().ToLowerInvariant()
        };
    }

    /// <summary>
    ///     Returns the urgency priority of this alert type (lower is more urgent).
    /// </summary>
    public static int Priority(this AlertType alertType)
    {
        return alertType switch
        {
            AlertType.Expired => 1,
            AlertType.OutOfStock => 2,
            AlertType.ExpiringSoon => 3,
            AlertType.LowStock => 4,
            _ => int.MaxValue
        };
    }
}
