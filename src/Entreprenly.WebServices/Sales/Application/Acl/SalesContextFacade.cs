using Entreprenly.WebServices.Sales.Application.CommandServices;
using Entreprenly.WebServices.Sales.Application.QueryServices;
using Entreprenly.WebServices.Sales.Domain.Model.Commands;
using Entreprenly.WebServices.Sales.Domain.Model.Queries;
using Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Sales.Interfaces.Acl;

namespace Entreprenly.WebServices.Sales.Application.Acl;

/// <summary>
///     Application-layer implementation of the Sales ACL facade.
/// </summary>
/// <remarks>
///     Registers a sale and updates the daily cash register, exposing the operation to other
///     bounded contexts without coupling them to the Sales internal model.
/// </remarks>
public class SalesContextFacade(
    ISaleCommandService saleCommandService,
    ICashRegisterCommandService cashRegisterCommandService,
    ICashRegisterQueryService cashRegisterQueryService,
    ILogger<SalesContextFacade> logger)
    : ISalesContextFacade
{
    public async Task<bool> RegisterChatSale(string ownerEmail, long sellerId, List<ChatSaleLine> lines, double total,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ownerEmail) || sellerId <= 0 || lines is null || lines.Count == 0)
            return false;

        // productId=0 is a chatbot-order sentinel: chatbot orders don't have a catalog product id,
        // only a name.
        var saleItems = lines
            .Select(line => SaleItem.Of(0L, line.ProductName, line.Quantity, null, line.UnitPrice))
            .ToList();

        var paymentReceipt = new PaymentReceipt(PaymentMethod.Yape, null, total, DateTimeOffset.UtcNow);
        var saleResult = await saleCommandService.Handle(new CreateSaleCommand(
            ownerEmail, sellerId, saleItems, PaymentMethod.Yape, paymentReceipt, SaleStatus.Completed),
            cancellationToken);

        if (saleResult.IsFailure)
        {
            logger.LogWarning("[sales-acl] could not register sale: {Message}", saleResult.Message);
            return false;
        }

        await UpdateCashRegister(ownerEmail, total, cancellationToken);
        return true;
    }

    private async Task UpdateCashRegister(string ownerEmail, double amount, CancellationToken cancellationToken)
    {
        var today = TodayInLima();
        var existing = await cashRegisterQueryService.Handle(new GetCashRegisterByDateQuery(ownerEmail, today),
            cancellationToken);

        if (existing is not null)
            await cashRegisterCommandService.Handle(new UpdateCashRegisterCommand(
                ownerEmail,
                existing.Id,
                existing.TotalCash,
                Math.Round((existing.TotalDigital + amount) * 100.0) / 100.0,
                existing.SaleCount + 1), cancellationToken);
        else
            await cashRegisterCommandService.Handle(new CreateCashRegisterCommand(
                ownerEmail, today, 0.0, amount), cancellationToken);
    }

    private static DateOnly TodayInLima()
    {
        try
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById("America/Lima");
            return DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(DateTimeOffset.UtcNow, timeZone).DateTime);
        }
        catch (TimeZoneNotFoundException)
        {
            // Lima has no daylight saving; fall back to a fixed UTC-5 offset.
            return DateOnly.FromDateTime(DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-5)).DateTime);
        }
    }
}
