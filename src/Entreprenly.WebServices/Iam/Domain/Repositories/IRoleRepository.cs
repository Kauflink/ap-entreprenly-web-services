using Entreprenly.WebServices.Iam.Domain.Model.Entities;
using Entreprenly.WebServices.Iam.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Iam.Domain.Repositories;

/// <summary>
///     Repository contract for the <see cref="Role" /> entity.
/// </summary>
public interface IRoleRepository : IBaseRepository<Role>
{
    Task<Role?> FindByNameAsync(ERoles name, CancellationToken cancellationToken);

    Task<bool> ExistsByNameAsync(ERoles name, CancellationToken cancellationToken);
}
