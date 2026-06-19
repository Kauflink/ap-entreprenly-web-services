using Entreprenly.WebServices.Sales.Application.CommandServices;
using Entreprenly.WebServices.Sales.Domain.Model.Commands;
using Entreprenly.WebServices.Sales.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Sales.Interfaces.Acl;

namespace Entreprenly.WebServices.Sales.Application.Acl;

/// <summary>
///     Application-layer implementation of the Sales ACL facade.
/// </summary>
/// <remarks>
///     Registers a sale coming from another bounded context (e.g. a confirmed chatbot order) without
///     coupling the caller to the Sales internal model. The day takings are derived from the
///     registered sales, so no separate cash-register update is needed here.
/// </remarks>
public class SalesContextFacade(
    ISaleCommandService saleCommandService,
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

        return true;
    }
}
