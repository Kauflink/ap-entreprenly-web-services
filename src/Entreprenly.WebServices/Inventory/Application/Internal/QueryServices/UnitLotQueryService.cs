using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Domain.Repositories;

namespace Entreprenly.WebServices.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Resolves unit lot read queries, scoped to the owner account.
/// </summary>
public class UnitLotQueryService(IUnitLotRepository unitLotRepository) : IUnitLotQueryService
{
    public async Task<IEnumerable<UnitLot>> Handle(GetAllUnitLotsQuery query, CancellationToken cancellationToken)
    {
        return await unitLotRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
    }

    public async Task<UnitLot?> Handle(GetUnitLotByIdQuery query, CancellationToken cancellationToken)
    {
        return await unitLotRepository.FindByIdAndOwnerEmailAsync(query.UnitLotId, query.OwnerEmail,
            cancellationToken);
    }
}
