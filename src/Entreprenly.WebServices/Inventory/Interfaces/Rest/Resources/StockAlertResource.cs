namespace Entreprenly.WebServices.Inventory.Interfaces.Rest.Resources;

/// <summary>
///     Resource representing a derived inventory stock alert returned by the REST API.
/// </summary>
public record StockAlertResource(
    int Id,
    int? LotId,
    int ProductId,
    string? ProductType,
    string ProductName,
    string AlertType,
    string Severity,
    string Message,
    DateTimeOffset CreatedAt);
