using Entreprenly.WebServices.Profiles.Domain.Model.Aggregates;
using Entreprenly.WebServices.Profiles.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Profiles.Interfaces.Rest.Transform;

public static class ProfileResourceFromEntityAssembler
{
    public static ProfileResource ToResourceFromEntity(Profile entity)
    {
        ArgumentNullException.ThrowIfNull(entity);
        return new ProfileResource(
            entity.Id,
            entity.UserId,
            entity.FirstName,
            entity.LastName,
            entity.Phone,
            entity.AvatarUrl,
            entity.Role,
            entity.Plan,
            new PreferencesResource(
                entity.Preferences.Language,
                entity.Preferences.Timezone,
                entity.Preferences.Theme,
                entity.Preferences.Currency),
            new NotificationSettingsResource(
                entity.NotificationSettings.StockAlerts));
    }
}
