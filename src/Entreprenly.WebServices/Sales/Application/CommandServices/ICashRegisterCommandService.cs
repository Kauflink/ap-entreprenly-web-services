using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;

namespace Entreprenly.WebServices.Sales.Application.CommandServices;

/// <summary>
///     Handles cash register commands.
/// </summary>
public interface ICashRegisterCommandService
{
    Task<Result<CashRegister>> Handle(CreateCashRegisterCommand command, CancellationToken cancellationToken);

    Task<Result<CashRegister>> Handle(UpdateCashRegisterCommand command, CancellationToken cancellationToken);
}
