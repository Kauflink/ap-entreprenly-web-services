using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Iam.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     EF Core repository for the <see cref="User" /> aggregate.
/// </summary>
public class UserRepository(AppDbContext context) : BaseRepository<User>(context), IUserRepository
{
    public override async Task<User?> FindByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .Include(user => user.Roles)
            .FirstOrDefaultAsync(user => user.Id == id, cancellationToken);
    }

    public override async Task<IEnumerable<User>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<User>()
            .Include(user => user.Roles)
            .ToListAsync(cancellationToken);
    }

    public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await Context.Set<User>()
            .Include(user => user.Roles)
            .FirstOrDefaultAsync(user => user.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await Context.Set<User>().AnyAsync(user => user.Email == email, cancellationToken);
    }
}
