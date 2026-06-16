using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Inventory.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="UnitLot" /> aggregate. All lookups are scoped to the
///     owner account.
/// </summary>
public interface IUnitLotRepository : IBaseRepository<UnitLot>
{
    Task<IEnumerable<UnitLot>> FindAllByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken);

    Task<UnitLot?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail, CancellationToken cancellationToken);

    Task<bool> ExistsByIdAndOwnerEmailAsync(int id, string ownerEmail, CancellationToken cancellationToken);

    Task RemoveByProductIdAndOwnerEmailAsync(int productId, string ownerEmail, CancellationToken cancellationToken);
}
