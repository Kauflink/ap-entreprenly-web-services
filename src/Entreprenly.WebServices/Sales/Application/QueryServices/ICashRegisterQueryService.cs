using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Model.Queries;

namespace Entreprenly.WebServices.Sales.Application.QueryServices;

/// <summary>
///     Handles cash register-related queries.
/// </summary>
public interface ICashRegisterQueryService
{
    Task<IEnumerable<CashRegister>> Handle(GetAllCashRegistersQuery query, CancellationToken cancellationToken);

    Task<CashRegister?> Handle(GetCashRegisterByDateQuery query, CancellationToken cancellationToken);
}
