using Entreprenly.WebServices.Inventory.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Inventory.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="UnitProduct" /> aggregate. All lookups are scoped to
///     the owner account.
/// </summary>
public interface IUnitProductRepository : IBaseRepository<UnitProduct>
{
    Task<IEnumerable<UnitProduct>> FindAllByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken);

    Task<UnitProduct?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail, CancellationToken cancellationToken);

    Task<bool> ExistsByIdAndOwnerEmailAsync(int id, string ownerEmail, CancellationToken cancellationToken);

    Task<bool> ExistsByCodeQrAndOwnerEmailAsync(string codeQr, string ownerEmail, CancellationToken cancellationToken);
}
