using Entreprenly.WebServices.Shared.Domain.Repositories;
using Entreprenly.WebServices.Subscription.Domain.Model.Aggregates;

namespace Entreprenly.WebServices.Subscription.Domain.Repositories;

public interface ISubscriptionRepository : IBaseRepository<Domain.Model.Aggregates.Subscription>
{
    Task<Domain.Model.Aggregates.Subscription?> FindByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<bool> ExistsByUserIdAsync(int userId, CancellationToken cancellationToken);
}
