using Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Sales.Interfaces.Rest.Transform;

/// <summary>
///     Maps Sales value-object enums to and from their wire representation. The wire format keeps the
///     uppercase snake-case names used by the original API (e.g. <c>CASH</c>, <c>IN_PROGRESS</c>) so
///     existing clients remain compatible.
/// </summary>
public static class SalesEnumMapper
{
    public static string ToWire(PaymentMethod method)
    {
        return method switch
        {
            PaymentMethod.Cash => "CASH",
            PaymentMethod.Yape => "YAPE",
            PaymentMethod.Plin => "PLIN",
            PaymentMethod.Card => "CARD",
            _ => "CASH"
        };
    }

    public static PaymentMethod? PaymentMethodFromWire(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return value.Trim().ToUpperInvariant() switch
        {
            "CASH" => PaymentMethod.Cash,
            "YAPE" => PaymentMethod.Yape,
            "PLIN" => PaymentMethod.Plin,
            "CARD" => PaymentMethod.Card,
            _ => null
        };
    }

    public static string ToWire(SaleStatus status)
    {
        return status switch
        {
            SaleStatus.InProgress => "IN_PROGRESS",
            SaleStatus.PaymentConfirmed => "PAYMENT_CONFIRMED",
            SaleStatus.Completed => "COMPLETED",
            SaleStatus.Cancelled => "CANCELLED",
            _ => "IN_PROGRESS"
        };
    }

    public static SaleStatus? SaleStatusFromWire(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        return value.Trim().ToUpperInvariant() switch
        {
            "IN_PROGRESS" => SaleStatus.InProgress,
            "PAYMENT_CONFIRMED" => SaleStatus.PaymentConfirmed,
            "COMPLETED" => SaleStatus.Completed,
            "CANCELLED" => SaleStatus.Cancelled,
            _ => null
        };
    }
}
