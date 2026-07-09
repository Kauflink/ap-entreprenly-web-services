using Entreprenly.WebServices.Sales.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Sales.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="Sale" /> aggregate. All lookups are scoped to the
///     owner account.
/// </summary>
public interface ISaleRepository : IBaseRepository<Sale>
{
    Task<IEnumerable<Sale>> FindAllByOwnerEmailAsync(string ownerEmail, CancellationToken cancellationToken);

    Task<IEnumerable<Sale>> FindAllByOwnerEmailAndDateAsync(string ownerEmail, DateOnly date,
        CancellationToken cancellationToken);

    Task<Sale?> FindByIdAndOwnerEmailAsync(int id, string ownerEmail, CancellationToken cancellationToken);
}
