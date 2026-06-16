using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model.Entities;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Inventory.Domain.Repositories;

namespace Entreprenly.WebServices.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Assembles the combined lot read view from the unit and weight lot aggregates owned by an
///     account.
/// </summary>
public class LotQueryService(IUnitLotRepository unitLotRepository, IWeightLotRepository weightLotRepository)
    : ILotQueryService
{
    public async Task<IEnumerable<Lot>> Handle(GetAllLotsQuery query, CancellationToken cancellationToken)
    {
        var lots = new List<Lot>();

        var unitLots = await unitLotRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
        lots.AddRange(unitLots.Select(lot =>
            new Lot(lot.Id, lot.ProductId, lot.CodeQr, lot.EntryDate, ProductType.Unit)));

        var weightLots = await weightLotRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
        lots.AddRange(weightLots.Select(lot =>
            new Lot(lot.Id, lot.ProductId, lot.CodeQr, lot.EntryDate, ProductType.Weight)));

        return lots;
    }

    public async Task<Lot?> Handle(GetLotByIdQuery query, CancellationToken cancellationToken)
    {
        var lots = await Handle(new GetAllLotsQuery(query.OwnerEmail), cancellationToken);
        return lots.FirstOrDefault(lot => lot.Id == query.LotId);
    }
}
