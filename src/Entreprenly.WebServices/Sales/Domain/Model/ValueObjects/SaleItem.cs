namespace Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;

/// <summary>
///     Line item of a sale. A sale item is sold either by unit (<see cref="Quantity" />) or by
///     weight (<see cref="WeightKg" />); the <see cref="Subtotal" /> is derived from whichever
///     applies.
/// </summary>
/// <param name="ProductId">The product identifier this line refers to.</param>
/// <param name="ProductName">The product display name captured at sale time.</param>
/// <param name="Quantity">The number of units sold (null when sold by weight).</param>
/// <param name="WeightKg">The weight sold in kilograms (null when sold by unit).</param>
/// <param name="UnitPrice">The price per unit or per kilogram.</param>
/// <param name="Subtotal">The computed line subtotal.</param>
public record SaleItem(
    long ProductId,
    string ProductName,
    int? Quantity,
    double? WeightKg,
    double UnitPrice,
    double Subtotal)
{
    /// <summary>
    ///     Creates a sale item computing its subtotal from the provided pricing data.
    /// </summary>
    public static SaleItem Of(long productId, string productName, int? quantity, double? weightKg, double unitPrice)
    {
        return new SaleItem(productId, productName, quantity, weightKg, unitPrice,
            ComputeSubtotal(quantity, weightKg, unitPrice));
    }

    /// <summary>
    ///     Computes the subtotal for a line, favouring weight-based pricing when present.
    /// </summary>
    public static double ComputeSubtotal(int? quantity, double? weightKg, double unitPrice)
    {
        double raw;
        if (weightKg is not null)
            raw = unitPrice * weightKg.Value;
        else if (quantity is not null)
            raw = unitPrice * quantity.Value;
        else
            raw = 0d;
        return Math.Round(raw * 100.0) / 100.0;
    }
}
