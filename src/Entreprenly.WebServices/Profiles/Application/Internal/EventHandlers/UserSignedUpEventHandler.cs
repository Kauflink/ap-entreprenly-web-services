using Entreprenly.WebServices.Iam.Interfaces.Events;
using Entreprenly.WebServices.Profiles.Application.CommandServices;
using Entreprenly.WebServices.Profiles.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Internal.EventHandlers;

namespace Entreprenly.WebServices.Profiles.Application.Internal.EventHandlers;

/// <summary>
///     Listens for <see cref="UserSignedUpIntegrationEvent" /> from the IAM context and provisions a
///     default profile for the newly registered user.
/// </summary>
public class UserSignedUpEventHandler(
    IServiceScopeFactory scopeFactory,
    ILogger<UserSignedUpEventHandler> logger)
    : IEventHandler<UserSignedUpIntegrationEvent>
{
    private const string DefaultRole = "User";
    private const string DefaultPlan = "Free";

    public async Task Handle(UserSignedUpIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var firstName = string.IsNullOrWhiteSpace(notification.FirstName)
            ? DeriveFirstName(notification.Email)
            : notification.FirstName;
        var lastName = notification.LastName ?? string.Empty;

        var command = new CreateProfileCommand(notification.UserId, firstName, lastName, DefaultRole, DefaultPlan,
            notification.Phone, notification.Timezone);

        // Resolve the command service from a dedicated scope so this handler uses its own DbContext
        // instead of sharing the request-scoped one with the other sign-up handlers. Sharing it makes
        // the concurrent handlers collide ("a second operation was started on this context instance").
        using var scope = scopeFactory.CreateScope();
        var profileCommandService = scope.ServiceProvider.GetRequiredService<IProfileCommandService>();

        var result = await profileCommandService.Handle(command, cancellationToken);
        if (result.IsFailure)
            logger.LogWarning("Could not auto-create profile for user {UserId}: a profile may already exist",
                notification.UserId);
        else
            logger.LogInformation("Default profile created for user {UserId}", notification.UserId);
    }

    private static string DeriveFirstName(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return string.Empty;
        var at = email.IndexOf('@');
        return at > 0 ? email[..at] : email;
    }
}
