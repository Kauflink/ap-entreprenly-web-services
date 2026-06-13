using Entreprenly.WebServices.Shared.Domain.Repositories;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;

namespace Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

/// <summary>
///     Unit of work for the application. Saves changes to the database context.
/// </summary>
/// <param name="context">The database context for the application.</param>
public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }
}
