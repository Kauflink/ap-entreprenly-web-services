using Entreprenly.WebServices.Iam.Interfaces.Events;
using Entreprenly.WebServices.Shared.Application.Internal.EventHandlers;
using Entreprenly.WebServices.Subscription.Application.CommandServices;
using Entreprenly.WebServices.Subscription.Domain.Model.Commands;

namespace Entreprenly.WebServices.Subscription.Application.Internal.EventHandlers;

public class UserSignedUpEventHandler(
    ISubscriptionCommandService subscriptionCommandService,
    ILogger<UserSignedUpEventHandler> logger)
    : IEventHandler<UserSignedUpIntegrationEvent>
{
    public async Task Handle(UserSignedUpIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var result = await subscriptionCommandService.Handle(new CreateSubscriptionCommand(notification.UserId),
            cancellationToken);

        if (result.IsFailure)
            logger.LogWarning("Could not auto-create subscription for user {UserId}: a subscription may already exist",
                notification.UserId);
        else
            logger.LogInformation("Default subscription created for user {UserId}", notification.UserId);
    }
}
