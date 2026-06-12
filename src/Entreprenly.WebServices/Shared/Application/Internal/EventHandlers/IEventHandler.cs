using Cortex.Mediator.Notifications;
using Entreprenly.WebServices.Shared.Domain.Model.Events;

namespace Entreprenly.WebServices.Shared.Application.Internal.EventHandlers;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IEvent
{
}
