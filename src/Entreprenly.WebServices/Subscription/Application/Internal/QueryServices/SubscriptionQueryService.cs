using Entreprenly.WebServices.Subscription.Application.QueryServices;
using Entreprenly.WebServices.Subscription.Domain.Model.Queries;
using Entreprenly.WebServices.Subscription.Domain.Repositories;

namespace Entreprenly.WebServices.Subscription.Application.Internal.QueryServices;

public class SubscriptionQueryService(ISubscriptionRepository subscriptionRepository) : ISubscriptionQueryService
{
    public async Task<Domain.Model.Aggregates.Subscription?> Handle(GetSubscriptionByUserIdQuery query,
        CancellationToken cancellationToken)
    {
        return await subscriptionRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }
}
