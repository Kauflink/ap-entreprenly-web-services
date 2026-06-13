using Entreprenly.WebServices.Profiles.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Profiles.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="Profile" /> aggregate.
/// </summary>
public interface IProfileRepository : IBaseRepository<Profile>
{
    Task<Profile?> FindByUserIdAsync(int userId, CancellationToken cancellationToken);

    Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken);
}
