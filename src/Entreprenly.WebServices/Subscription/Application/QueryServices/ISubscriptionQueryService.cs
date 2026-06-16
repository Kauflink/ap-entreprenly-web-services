using Entreprenly.WebServices.Subscription.Domain.Model.Queries;

namespace Entreprenly.WebServices.Subscription.Application.QueryServices;

public interface ISubscriptionQueryService
{
    Task<Domain.Model.Aggregates.Subscription?> Handle(GetSubscriptionByUserIdQuery query,
        CancellationToken cancellationToken);
}
