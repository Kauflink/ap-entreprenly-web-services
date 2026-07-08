using Entreprenly.WebServices.Sales.Application.CommandServices;
using Entreprenly.WebServices.Sales.Domain.Model;
using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Model.Commands;
using Entreprenly.WebServices.Sales.Domain.Repositories;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Sales.Application.Internal.CommandServices;

/// <summary>
///     Handles sale commands.
/// </summary>
public class SaleCommandService(
    ISaleRepository saleRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : ISaleCommandService
{
    public async Task<Result<Sale>> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.OwnerEmail))
            return Result<Sale>.Failure(SalesError.OwnerRequired, localizer[nameof(SalesError.OwnerRequired)]);

        if (command.SellerId <= 0)
            return Result<Sale>.Failure(SalesError.SellerRequired, localizer[nameof(SalesError.SellerRequired)]);

        if (command.Items is null || command.Items.Count == 0)
            return Result<Sale>.Failure(SalesError.SaleItemsRequired, localizer[nameof(SalesError.SaleItemsRequired)]);

        var sale = new Sale(command.OwnerEmail, command.SellerId, command.Items, command.PaymentMethod,
            command.PaymentReceipt, command.Status);

        await saleRepository.AddAsync(sale, cancellationToken);
        return await CompleteAsync(sale, cancellationToken);
    }

    private async Task<Result<Sale>> CompleteAsync(Sale sale, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Sale>.Success(sale);
        }
        catch (OperationCanceledException)
        {
            return Result<Sale>.Failure(SalesError.OperationCancelled, localizer[nameof(SalesError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Sale>.Failure(SalesError.DatabaseError, localizer[nameof(SalesError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<Sale>.Failure(SalesError.InternalServerError,
                localizer[nameof(SalesError.InternalServerError)]);
        }
    }
}
