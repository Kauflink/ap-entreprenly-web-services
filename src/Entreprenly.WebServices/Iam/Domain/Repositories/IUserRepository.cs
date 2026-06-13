using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Iam.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="User" /> aggregate.
/// </summary>
public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);

    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
}
