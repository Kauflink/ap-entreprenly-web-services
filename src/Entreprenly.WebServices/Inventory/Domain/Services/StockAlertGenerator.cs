using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Entities;
using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Inventory.Domain.Services;

/// <summary>
///     Domain service that derives <see cref="StockAlert" /> read models from the current state of
///     inventory products and their lots. Alerts are returned ordered from most to least urgent and
///     carry stable sequential identifiers within a generation run.
/// </summary>
public static class StockAlertGenerator
{
    private const int ExpiringSoonDays = 5;
    private const int LowUnitStockThreshold = 5;
    private const double LowWeightStockThreshold = 5d;

    /// <summary>
    ///     Generates the stock alerts implied by the supplied inventory snapshot.
    /// </summary>
    /// <param name="unitProducts">The registered unit products.</param>
    /// <param name="weightProducts">The registered weight products.</param>
    /// <param name="unitLots">The registered unit lots.</param>
    /// <param name="weightLots">The registered weight lots.</param>
    /// <param name="now">The reference instant used for expiry calculations.</param>
    /// <returns>The derived alerts ordered by descending urgency, with sequential ids.</returns>
    public static List<StockAlert> Generate(IEnumerable<UnitProduct> unitProducts,
        IEnumerable<WeightProduct> weightProducts, IEnumerable<UnitLot> unitLots,
        IEnumerable<WeightLot> weightLots, DateTimeOffset now)
    {
        var alerts = new List<StockAlert>();
        var today = now.UtcDateTime.Date;
        var unitLotList = unitLots.ToList();
        var weightLotList = weightLots.ToList();

        foreach (var product in unitProducts)
        {
            var productLots = unitLotList.Where(l => l.ProductId == product.Id).ToList();
            var totalStock = productLots.Sum(l => l.Quantity);
            var outLots = productLots.Where(l => l.Quantity <= 0).ToList();

            foreach (var lot in outLots)
                alerts.Add(MakeAlert(AlertType.OutOfStock, product.Id, product.Name, ProductType.Unit, lot.Id, now));

            if (outLots.Count == 0 && totalStock <= 0)
                alerts.Add(MakeAlert(AlertType.OutOfStock, product.Id, product.Name, ProductType.Unit, null, now));

            if (totalStock > 0 && totalStock <= LowUnitStockThreshold)
            {
                int? lotId = productLots.Count == 0 ? null : productLots[0].Id;
                alerts.Add(MakeAlert(AlertType.LowStock, product.Id, product.Name, ProductType.Unit, lotId, now));
            }

            foreach (var lot in productLots)
            {
                if (lot.ExpiryDate is null) continue;
                var expiry = lot.ExpiryDate.Value.UtcDateTime.Date;
                var days = (expiry - today).Days;
                if (days < 0)
                    alerts.Add(MakeAlert(AlertType.Expired, product.Id, product.Name, ProductType.Unit, lot.Id, now));
                else if (days <= ExpiringSoonDays)
                    alerts.Add(MakeAlert(AlertType.ExpiringSoon, product.Id, product.Name, ProductType.Unit, lot.Id,
                        now));
            }
        }

        foreach (var product in weightProducts)
        {
            var productLots = weightLotList.Where(l => l.ProductId == product.Id).ToList();
            var totalStock = productLots.Sum(l => l.QuantityKg);
            var outLots = productLots.Where(l => l.QuantityKg <= 0).ToList();

            foreach (var lot in outLots)
                alerts.Add(MakeAlert(AlertType.OutOfStock, product.Id, product.Name, ProductType.Weight, lot.Id, now));

            if (outLots.Count == 0 && totalStock <= 0)
                alerts.Add(MakeAlert(AlertType.OutOfStock, product.Id, product.Name, ProductType.Weight, null, now));

            if (totalStock > 0 && totalStock <= LowWeightStockThreshold)
            {
                int? lotId = productLots.Count == 0 ? null : productLots[0].Id;
                alerts.Add(MakeAlert(AlertType.LowStock, product.Id, product.Name, ProductType.Weight, lotId, now));
            }
        }

        var ordered = alerts.OrderBy(a => a.AlertType.Priority()).ToList();

        var sequence = 1;
        return ordered.Select(alert => alert.WithId(sequence++)).ToList();
    }

    private static StockAlert MakeAlert(AlertType alertType, int productId, string productName,
        ProductType productType, int? lotId, DateTimeOffset now)
    {
        var severity = alertType is AlertType.Expired or AlertType.OutOfStock
            ? AlertSeverity.Critical
            : AlertSeverity.Warning;
        return new StockAlert(0, lotId, productId, productType, productName, alertType, severity,
            BuildMessage(alertType, productName), now);
    }

    private static string BuildMessage(AlertType alertType, string? productName)
    {
        var name = string.IsNullOrEmpty(productName) ? "Product" : productName;
        return alertType switch
        {
            AlertType.Expired => $"{name} has an expired lot",
            AlertType.OutOfStock => $"{name} is out of stock",
            AlertType.ExpiringSoon => $"{name} has a lot expiring soon",
            AlertType.LowStock => $"{name} is running low on stock",
            _ => $"{name} has a stock alert"
        };
    }
}
