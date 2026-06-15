using Entreprenly.WebServices.Sales.Application.QueryServices;
using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Model.Queries;
using Entreprenly.WebServices.Sales.Domain.Repositories;

namespace Entreprenly.WebServices.Sales.Application.Internal.QueryServices;

/// <summary>
///     Handles cash register queries.
/// </summary>
public class CashRegisterQueryService(ICashRegisterRepository cashRegisterRepository) : ICashRegisterQueryService
{
    public async Task<IEnumerable<CashRegister>> Handle(GetAllCashRegistersQuery query,
        CancellationToken cancellationToken)
    {
        return await cashRegisterRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
    }

    public async Task<CashRegister?> Handle(GetCashRegisterByDateQuery query, CancellationToken cancellationToken)
    {
        return await cashRegisterRepository.FindByDateAndOwnerEmailAsync(query.Date, query.OwnerEmail,
            cancellationToken);
    }
}
