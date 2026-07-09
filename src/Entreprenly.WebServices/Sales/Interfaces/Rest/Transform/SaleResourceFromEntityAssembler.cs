using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;

public static class SaleResourceFromEntityAssembler
{
    public static SaleResource ToResourceFromEntity(Sale entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        var items = entity.Items
            .Select(item => new SaleItemResource(
                item.ProductId,
                item.ProductName,
                item.Quantity,
                item.WeightKg,
                item.UnitPrice,
                item.Subtotal))
            .ToList();

        return new SaleResource(
            entity.Id,
            entity.SellerId,
            items,
            entity.Total,
            SalesEnumMapper.ToWire(entity.PaymentMethod),
            ToPaymentReceiptResource(entity.PaymentReceipt),
            SalesEnumMapper.ToWire(entity.Status),
            entity.CreatedAt,
            entity.CompletedAt);
    }

    private static PaymentReceiptResource? ToPaymentReceiptResource(PaymentReceipt? receipt)
    {
        if (receipt is null) return null;
        return new PaymentReceiptResource(
            SalesEnumMapper.ToWire(receipt.Method),
            receipt.TransactionCode,
            receipt.Amount,
            receipt.ConfirmedAt);
    }
}
