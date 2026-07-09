using Entreprenly.WebServices.Sales.Domain.Model.Commands;
using Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Sales.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;

/// <summary>
///     Translates a <see cref="CreateSaleResource" /> into a <see cref="CreateSaleCommand" />,
///     recomputing each line's subtotal from its pricing data.
/// </summary>
public static class CreateSaleCommandFromResourceAssembler
{
    public static CreateSaleCommand ToCommandFromResource(string ownerEmail, CreateSaleResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);

        var items = (resource.Items ?? [])
            .Select(item => SaleItem.Of(item.ProductId, item.ProductName, item.Quantity, item.WeightKg,
                item.UnitPrice))
            .ToList();

        var paymentReceipt = ToPaymentReceipt(resource.PaymentReceipt);

        return new CreateSaleCommand(
            ownerEmail,
            resource.SellerId,
            items,
            SalesEnumMapper.PaymentMethodFromWire(resource.PaymentMethod),
            paymentReceipt,
            SalesEnumMapper.SaleStatusFromWire(resource.Status));
    }

    private static PaymentReceipt? ToPaymentReceipt(PaymentReceiptResource? resource)
    {
        if (resource is null) return null;
        var method = SalesEnumMapper.PaymentMethodFromWire(resource.Method) ?? PaymentMethod.Cash;
        var confirmedAt = resource.ConfirmedAt == default ? DateTimeOffset.UtcNow : resource.ConfirmedAt;
        return new PaymentReceipt(method, resource.TransactionCode, resource.Amount, confirmedAt);
    }
}
