using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Domain.Repositories;

namespace Entreprenly.WebServices.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Resolves weight lot read queries, scoped to the owner account.
/// </summary>
public class WeightLotQueryService(IWeightLotRepository weightLotRepository) : IWeightLotQueryService
{
    public async Task<IEnumerable<WeightLot>> Handle(GetAllWeightLotsQuery query, CancellationToken cancellationToken)
    {
        return await weightLotRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
    }

    public async Task<WeightLot?> Handle(GetWeightLotByIdQuery query, CancellationToken cancellationToken)
    {
        return await weightLotRepository.FindByIdAndOwnerEmailAsync(query.WeightLotId, query.OwnerEmail,
            cancellationToken);
    }
}
