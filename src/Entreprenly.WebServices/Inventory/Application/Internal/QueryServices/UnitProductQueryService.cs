using Entreprenly.WebServices.Inventory.Application.QueryServices;
using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;
using Entreprenly.WebServices.Inventory.Domain.Repositories;

namespace Entreprenly.WebServices.Inventory.Application.Internal.QueryServices;

/// <summary>
///     Resolves unit product read queries, scoped to the owner account.
/// </summary>
public class UnitProductQueryService(IUnitProductRepository unitProductRepository) : IUnitProductQueryService
{
    public async Task<IEnumerable<UnitProduct>> Handle(GetAllUnitProductsQuery query,
        CancellationToken cancellationToken)
    {
        return await unitProductRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
    }

    public async Task<UnitProduct?> Handle(GetUnitProductByIdQuery query, CancellationToken cancellationToken)
    {
        return await unitProductRepository.FindByIdAndOwnerEmailAsync(query.UnitProductId, query.OwnerEmail,
            cancellationToken);
    }
}
