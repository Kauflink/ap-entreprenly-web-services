using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;

namespace Entreprenly.WebServices.Inventory.Application.QueryServices;

/// <summary>
///     Handles unit product queries.
/// </summary>
public interface IUnitProductQueryService
{
    Task<IEnumerable<UnitProduct>> Handle(GetAllUnitProductsQuery query, CancellationToken cancellationToken);

    Task<UnitProduct?> Handle(GetUnitProductByIdQuery query, CancellationToken cancellationToken);
}
