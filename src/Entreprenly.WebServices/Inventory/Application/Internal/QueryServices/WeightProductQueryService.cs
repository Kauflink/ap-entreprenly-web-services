using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Domain.Repositories;

namespace Entreprenly.WebServices.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Resolves weight product read queries, scoped to the owner account.
/// </summary>
public class WeightProductQueryService(IWeightProductRepository weightProductRepository) : IWeightProductQueryService
{
    public async Task<IEnumerable<WeightProduct>> Handle(GetAllWeightProductsQuery query,
        CancellationToken cancellationToken)
    {
        return await weightProductRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
    }

    public async Task<WeightProduct?> Handle(GetWeightProductByIdQuery query, CancellationToken cancellationToken)
    {
        return await weightProductRepository.FindByIdAndOwnerEmailAsync(query.WeightProductId, query.OwnerEmail,
            cancellationToken);
    }
}
