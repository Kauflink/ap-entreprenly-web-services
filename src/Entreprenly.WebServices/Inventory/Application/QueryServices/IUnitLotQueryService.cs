using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;

namespace Entreprenly.WebServices.Inventory.Application.QueryServices;

/// <summary>
///     Handles unit lot queries.
/// </summary>
public interface IUnitLotQueryService
{
    Task<IEnumerable<UnitLot>> Handle(GetAllUnitLotsQuery query, CancellationToken cancellationToken);

    Task<UnitLot?> Handle(GetUnitLotByIdQuery query, CancellationToken cancellationToken);
}
