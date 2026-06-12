using Entreprenly.WebServices.Shared.Domain.Model.Events;

namespace Entreprenly.WebServices.Iam.Domain.Model.Events;

/// <summary>
///     Domain event published when a new user successfully signs up. Consumed by other bounded
///     contexts (e.g. Profiles) to provision dependent data without coupling to IAM internals.
/// </summary>
public record UserSignedUpEvent(
    int UserId,
    string Email,
    string? FirstName,
    string? LastName,
    string? Phone,
    string? Timezone) : IEvent;
