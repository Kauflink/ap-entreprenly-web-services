using Entreprenly.WebServices.Iam.Domain.Model.Entities;
using Entreprenly.WebServices.Iam.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Iam.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the <see cref="Role" /> entity.
/// </summary>
public class RoleRepository(AppDbContext context) : BaseRepository<Role>(context), IRoleRepository
{
    public async Task<Role?> FindByNameAsync(ERoles name, CancellationToken cancellationToken)
    {
        return await Context.Set<Role>().FirstOrDefaultAsync(role => role.Name == name, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(ERoles name, CancellationToken cancellationToken)
    {
        return await Context.Set<Role>().AnyAsync(role => role.Name == name, cancellationToken);
    }
}
