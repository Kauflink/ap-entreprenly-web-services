using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model.Entities;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Domain.Repositories;
using Entreprenly.WebServices.Inventory.Domain.Services;

namespace Entreprenly.WebServices.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Derives stock alerts from the current inventory snapshot of an account.
/// </summary>
public class StockAlertQueryService(
    IUnitProductRepository unitProductRepository,
    IWeightProductRepository weightProductRepository,
    IUnitLotRepository unitLotRepository,
    IWeightLotRepository weightLotRepository)
    : IStockAlertQueryService
{
    public async Task<IEnumerable<StockAlert>> Handle(GetAllStockAlertsQuery query,
        CancellationToken cancellationToken)
    {
        var unitProducts = await unitProductRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
        var weightProducts =
            await weightProductRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
        var unitLots = await unitLotRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
        var weightLots = await weightLotRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);

        return StockAlertGenerator.Generate(unitProducts, weightProducts, unitLots, weightLots,
            DateTimeOffset.UtcNow);
    }

    public async Task<StockAlert?> Handle(GetStockAlertByIdQuery query, CancellationToken cancellationToken)
    {
        var alerts = await Handle(new GetAllStockAlertsQuery(query.OwnerEmail), cancellationToken);
        return alerts.FirstOrDefault(alert => alert.Id == query.StockAlertId);
    }
}
