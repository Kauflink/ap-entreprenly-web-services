using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;

namespace Entreprenly.WebServices.Inventory.Application.QueryServices;

/// <summary>
///     Handles weight lot queries.
/// </summary>
public interface IWeightLotQueryService
{
    Task<IEnumerable<WeightLot>> Handle(GetAllWeightLotsQuery query, CancellationToken cancellationToken);

    Task<WeightLot?> Handle(GetWeightLotByIdQuery query, CancellationToken cancellationToken);
}
