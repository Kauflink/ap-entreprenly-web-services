using Entreprenly.WebServices.Profiles.Application.CommandServices;
using Entreprenly.WebServices.Profiles.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Entreprenly.WebServices.Subscription.Application.CommandServices;
using Entreprenly.WebServices.Subscription.Domain.Model;
using Entreprenly.WebServices.Subscription.Domain.Model.Commands;
using Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Subscription.Domain.Repositories;
using Entreprenly.WebServices.Subscription.Domain.Model.Errors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Subscription.Application.Internal.CommandServices;

public class SubscriptionCommandService(
    ISubscriptionRepository subscriptionRepository,
    IProfileCommandService profileCommandService,
    IUnitOfWork unitOfWork)
    : ISubscriptionCommandService
{
    public async Task<Result<Domain.Model.Aggregates.Subscription>> Handle(CreateSubscriptionCommand command,
        CancellationToken cancellationToken)
    {
        if (await subscriptionRepository.ExistsByUserIdAsync(command.UserId, cancellationToken))
            return Result<Domain.Model.Aggregates.Subscription>.Failure(SubscriptionError.SubscriptionAlreadyExists,
                SubscriptionErrors.SubscriptionAlreadyExists.Message);

        var subscription = new Domain.Model.Aggregates.Subscription(command.UserId);
        await subscriptionRepository.AddAsync(subscription, cancellationToken);
        return await CompleteAsync(subscription, cancellationToken);
    }

    public async Task<Result<Domain.Model.Aggregates.Subscription>> Handle(ActivateControlPlanCommand command,
        CancellationToken cancellationToken)
    {
        if (!BillingCycle.IsSupported(command.BillingCycle))
            return Result<Domain.Model.Aggregates.Subscription>.Failure(SubscriptionError.InvalidBillingCycle,
                SubscriptionErrors.InvalidBillingCycle.Message);

        var subscription = await GetOrCreateAsync(command.UserId, cancellationToken);
        subscription.ActivateControlPlan(command.BillingCycle, DateOnly.FromDateTime(DateTime.UtcNow));
        subscriptionRepository.Update(subscription);

        var result = await CompleteAsync(subscription, cancellationToken);
        if (result.IsSuccess)
            await profileCommandService.Handle(new UpdateProfilePlanCommand(command.UserId, "Plan Control"),
                cancellationToken);

        return result;
    }

    public async Task<Result<Domain.Model.Aggregates.Subscription>> Handle(
        ScheduleSubscriptionCancellationCommand command, CancellationToken cancellationToken)
    {
        var subscription = await GetOrCreateAsync(command.UserId, cancellationToken);
        subscription.ScheduleCancellation();
        subscriptionRepository.Update(subscription);
        return await CompleteAsync(subscription, cancellationToken);
    }

    public async Task<Result<Domain.Model.Aggregates.Subscription>> Handle(KeepControlPlanCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await GetOrCreateAsync(command.UserId, cancellationToken);
        subscription.KeepControlPlan();
        subscriptionRepository.Update(subscription);
        return await CompleteAsync(subscription, cancellationToken);
    }

    public async Task<Result<Domain.Model.Aggregates.Subscription>> Handle(UpdateBillingSetupCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await GetOrCreateAsync(command.UserId, cancellationToken);
        subscription.UpdateBillingSetup(command.BillingSetup);
        subscriptionRepository.Update(subscription);
        return await CompleteAsync(subscription, cancellationToken);
    }

    public async Task<Result<Domain.Model.Aggregates.Subscription>> Handle(ReplaceSubscriptionDashboardCommand command,
        CancellationToken cancellationToken)
    {
        var subscription = await GetOrCreateAsync(command.UserId, cancellationToken);
        subscription.ReplaceDashboard(command.DefaultBillingCycle, command.CurrentPlan, command.RecommendedPlan,
            command.Limits, command.BillingSetup, command.Activity);
        subscriptionRepository.Update(subscription);

        var result = await CompleteAsync(subscription, cancellationToken);
        if (result.IsSuccess && command.CurrentPlan.PlanId == "plan-control")
            await profileCommandService.Handle(new UpdateProfilePlanCommand(command.UserId, "Plan Control"),
                cancellationToken);

        return result;
    }

    private async Task<Domain.Model.Aggregates.Subscription> GetOrCreateAsync(int userId,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionRepository.FindByUserIdAsync(userId, cancellationToken);
        if (subscription is not null) return subscription;

        subscription = new Domain.Model.Aggregates.Subscription(userId);
        await subscriptionRepository.AddAsync(subscription, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);
        return subscription;
    }

    private async Task<Result<Domain.Model.Aggregates.Subscription>> CompleteAsync(
        Domain.Model.Aggregates.Subscription subscription, CancellationToken cancellationToken)
    {
        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Domain.Model.Aggregates.Subscription>.Success(subscription);
        }
        catch (OperationCanceledException)
        {
            return Result<Domain.Model.Aggregates.Subscription>.Failure(SubscriptionError.OperationCancelled,
                SubscriptionErrors.OperationCancelled.Message);
        }
        catch (DbUpdateException)
        {
            return Result<Domain.Model.Aggregates.Subscription>.Failure(SubscriptionError.DatabaseError,
                SubscriptionErrors.DatabaseError.Message);
        }
        catch (Exception)
        {
            return Result<Domain.Model.Aggregates.Subscription>.Failure(SubscriptionError.InternalServerError,
                SubscriptionErrors.InternalServerError.Message);
        }
    }
}
