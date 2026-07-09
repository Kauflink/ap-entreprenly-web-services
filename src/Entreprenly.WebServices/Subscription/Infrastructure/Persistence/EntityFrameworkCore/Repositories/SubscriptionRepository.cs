using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Configuration;
using Entreprenly.WebServices.Shared.Infrastructure.Persistence.EntityFrameworkCore.Repositories;
using Entreprenly.WebServices.Subscription.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Subscription.Infrastructure.Persistence.EntityFrameworkCore.Repositories;

public class SubscriptionRepository(AppDbContext context)
    : BaseRepository<Domain.Model.Aggregates.Subscription>(context), ISubscriptionRepository
{
    public async Task<Domain.Model.Aggregates.Subscription?> FindByUserIdAsync(int userId,
        CancellationToken cancellationToken)
    {
        return await Context.Set<Domain.Model.Aggregates.Subscription>()
            .FirstOrDefaultAsync(subscription => subscription.UserId == userId, cancellationToken);
    }

    public async Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await Context.Set<Domain.Model.Aggregates.Subscription>()
            .AnyAsync(subscription => subscription.UserId == userId, cancellationToken);
    }
}
