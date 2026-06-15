using Entreprenly.WebServices.Sales.Application.CommandServices;
using Entreprenly.WebServices.Sales.Domain.Model;
using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Model.Commands;
using Entreprenly.WebServices.Sales.Domain.Repositories;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Sales.Application.Internal.CommandServices;

/// <summary>
///     Handles cash register commands.
/// </summary>
public class CashRegisterCommandService(
    ICashRegisterRepository cashRegisterRepository,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : ICashRegisterCommandService
{
    public async Task<Result<CashRegister>> Handle(CreateCashRegisterCommand command,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.OwnerEmail))
            return Result<CashRegister>.Failure(SalesError.OwnerRequired,
                localizer[nameof(SalesError.OwnerRequired)]);

        if (command.Date == default)
            return Result<CashRegister>.Failure(SalesError.BusinessDayRequired,
                localizer[nameof(SalesError.BusinessDayRequired)]);

        if (await cashRegisterRepository.ExistsByDateAndOwnerEmailAsync(command.Date, command.OwnerEmail,
                cancellationToken))
            return Result<CashRegister>.Failure(SalesError.CashRegisterAlreadyExists,
                localizer[nameof(SalesError.CashRegisterAlreadyExists)]);

        var cashRegister = new CashRegister(command.OwnerEmail, command.Date, command.TotalCash,
            command.TotalDigital, 0);

        await cashRegisterRepository.AddAsync(cashRegister, cancellationToken);
        return await CompleteAsync(cashRegister, cancellationToken);
    }

    public async Task<Result<CashRegister>> Handle(UpdateCashRegisterCommand command,
        CancellationToken cancellationToken)
    {
        var cashRegister = await cashRegisterRepository.FindByIdAndOwnerEmailAsync(command.CashRegisterId,
            command.OwnerEmail, cancellationToken);
        if (cashRegister is null)
            return Result<CashRegister>.Failure(SalesError.CashRegisterNotFound,
                localizer[nameof(SalesError.CashRegisterNotFound)]);

        cashRegister.UpdateTotals(command.TotalCash, command.TotalDigital, command.SaleCount);
        cashRegisterRepository.Update(cashRegister);
        return await CompleteAsync(cashRegister, cancellationToken);
    }

    private async Task<Result<CashRegister>> CompleteAsync(CashRegister cashRegister,
        CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<CashRegister>.Success(cashRegister);
        }
        catch (OperationCanceledException)
        {
            return Result<CashRegister>.Failure(SalesError.OperationCancelled,
                localizer[nameof(SalesError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<CashRegister>.Failure(SalesError.DatabaseError,
                localizer[nameof(SalesError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<CashRegister>.Failure(SalesError.InternalServerError,
                localizer[nameof(SalesError.InternalServerError)]);
        }
    }
}
