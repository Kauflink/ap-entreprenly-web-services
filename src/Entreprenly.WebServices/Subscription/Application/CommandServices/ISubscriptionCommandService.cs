using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Subscription.Domain.Model.Aggregates;
using Entreprenly.WebServices.Subscription.Domain.Model.Commands;

namespace Entreprenly.WebServices.Subscription.Application.CommandServices;

public interface ISubscriptionCommandService
{
    Task<Result<Domain.Model.Aggregates.Subscription>> Handle(CreateSubscriptionCommand command,
        CancellationToken cancellationToken);

    Task<Result<Domain.Model.Aggregates.Subscription>> Handle(ActivateControlPlanCommand command,
        CancellationToken cancellationToken);

    Task<Result<Domain.Model.Aggregates.Subscription>> Handle(ScheduleSubscriptionCancellationCommand command,
        CancellationToken cancellationToken);

    Task<Result<Domain.Model.Aggregates.Subscription>> Handle(KeepControlPlanCommand command,
        CancellationToken cancellationToken);

    Task<Result<Domain.Model.Aggregates.Subscription>> Handle(UpdateBillingSetupCommand command,
        CancellationToken cancellationToken);

    Task<Result<Domain.Model.Aggregates.Subscription>> Handle(ReplaceSubscriptionDashboardCommand command,
        CancellationToken cancellationToken);
}
