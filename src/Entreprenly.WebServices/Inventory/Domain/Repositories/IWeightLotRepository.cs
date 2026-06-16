using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Inventory.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="WeightLot" /> aggregate. All lookups are scoped to the
///     owner account.
/// </summary>
public interface IWeightLotRepository : IBaseRepository<WeightLot>
{
    Task<IEnumerable<WeightLot>> FindAllByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken);

    Task<WeightLot?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail, CancellationToken cancellationToken);

    Task<bool> ExistsByIdAndOwnerEmailAsync(int id, string ownerEmail, CancellationToken cancellationToken);

    Task RemoveByProductIdAndOwnerEmailAsync(int productId, string ownerEmail, CancellationToken cancellationToken);
}
