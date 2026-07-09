using Entreprenly.WebServices.Iam.Domain.Model.Events;
using Entreprenly.WebServices.Shared.Domain.Model.Events;

namespace Entreprenly.WebServices.Iam.Interfaces.Events;

/// <summary>
///     Integration event published by the IAM context when a new user has signed up. This is the
///     published language of IAM: other bounded contexts subscribe to this event rather than to the
///     internal <see cref="UserSignedUpEvent" /> domain event.
/// </summary>
public record UserSignedUpIntegrationEvent(
    int UserId,
    string Email,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? Timezone) : IEvent
{
    /// <summary>
    ///     Maps the internal domain event to the integration event.
    /// </summary>
    public static UserSignedUpIntegrationEvent From(UserSignedUpEvent @event)
    {
        return new UserSignedUpIntegrationEvent(@event.UserId, @event.Email, @event.FirstName, @event.LastName,
            @event.Phone, @event.Timezone);
    }
}
