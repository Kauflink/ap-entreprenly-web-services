using Entreprenly.WebServices.Sales.Application.QueryServices;
using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Model.Queries;
using Entreprenly.WebServices.Sales.Domain.Repositories;

namespace Entreprenly.WebServices.Sales.Application.Internal.QueryServices;

/// <summary>
///     Handles sale queries.
/// </summary>
public class SaleQueryService(ISaleRepository saleRepository) : ISaleQueryService
{
    public async Task<IEnumerable<Sale>> Handle(GetAllSalesQuery query, CancellationToken cancellationToken)
    {
        return await saleRepository.FindAllByOwnerEmailAsync(query.OwnerEmail, cancellationToken);
    }

    public async Task<Sale?> Handle(GetSaleByIdQuery query, CancellationToken cancellationToken)
    {
        return await saleRepository.FindByIdAndOwnerEmailAsync(query.SaleId, query.OwnerEmail, cancellationToken);
    }
}
