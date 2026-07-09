using Entreprenly.WebServices.Inventory.Domain.Model.Entities;
using Entreprenly.WebServices.Inventory.Domain.Model.Queries;

namespace Entreprenly.WebServices.Inventory.Application.QueryServices;

/// <summary>
///     Handles combined (unit + weight) lot read queries.
/// </summary>
public interface ILotQueryService
{
    Task<IEnumerable<Lot>> Handle(GetAllLotsQuery query, CancellationToken cancellationToken);

    Task<Lot?> Handle(GetLotByIdQuery query, CancellationToken cancellationToken);
}
