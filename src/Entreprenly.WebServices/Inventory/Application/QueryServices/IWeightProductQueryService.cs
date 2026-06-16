using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;

namespace Entreprenly.WebServices.Inventory.Application.QueryServices;

/// <summary>
///     Handles weight product queries.
/// </summary>
public interface IWeightProductQueryService
{
    Task<IEnumerable<WeightProduct>> Handle(GetAllWeightProductsQuery query, CancellationToken cancellationToken);

    Task<WeightProduct?> Handle(GetWeightProductByIdQuery query, CancellationToken cancellationToken);
}
