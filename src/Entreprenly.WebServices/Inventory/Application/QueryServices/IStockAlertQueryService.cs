using Entreprenly.WebServices.Inventory.Domain.Model.Entities;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;

namespace Entreprenly.WebServices.Inventory.Application.QueryServices;

/// <summary>
///     Handles derived stock alert queries.
/// </summary>
public interface IStockAlertQueryService
{
    Task<IEnumerable<StockAlert>> Handle(GetAllStockAlertsQuery query, CancellationToken cancellationToken);

    Task<StockAlert?> Handle(GetStockAlertByIdQuery query, CancellationToken cancellationToken);
}
