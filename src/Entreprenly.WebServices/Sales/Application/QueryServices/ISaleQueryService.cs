using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Sales.Domain.Model.Queries;

namespace Entreprenly.WebServices.Sales.Application.QueryServices;

/// <summary>
///     Handles sale-related queries.
/// </summary>
public interface ISaleQueryService
{
    Task<IEnumerable<Sale>> Handle(GetAllSalesQuery query, CancellationToken cancellationToken);

    Task<Sale?> Handle(GetSaleByIdQuery query, CancellationToken cancellationToken);
}
