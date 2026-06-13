using Cortex.Mediator;
using Entreprenly.WebServices.Iam.Domain.Model.Events;
using Entreprenly.WebServices.Iam.Interfaces.Events;
using Entreprenly.WebServices.Shared.Application.Internal.EventHandlers;

namespace Entreprenly.WebServices.Iam.Application.Internal.EventHandlers;

/// <summary>
///     Translates the internal <see cref="UserSignedUpEvent" /> domain event into the
///     <see cref="UserSignedUpIntegrationEvent" /> published for cross-context consumers. Other
///     bounded contexts subscribe to the integration event, never to the internal domain event.
/// </summary>
public class UserSignedUpEventHandler(IMediator mediator) : IEventHandler<UserSignedUpEvent>
{
    public async Task Handle(UserSignedUpEvent notification, CancellationToken cancellationToken)
    {
        await mediator.PublishAsync(UserSignedUpIntegrationEvent.From(notification), cancellationToken);
    }
}
