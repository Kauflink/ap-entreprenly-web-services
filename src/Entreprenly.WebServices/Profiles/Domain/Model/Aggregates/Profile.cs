using Entreprenly.WebServices.Profiles.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Profiles.Domain.Model.Aggregates;

/// <summary>
///     Profile aggregate root. Holds the user-facing data for an IAM user (referenced by
///     <see cref="UserId" />), together with the user's <see cref="Preferences" /> and
///     <see cref="NotificationSettings" />.
/// </summary>
public partial class Profile
{
    public Profile()
    {
        FirstName = string.Empty;
        LastName = string.Empty;
        Role = string.Empty;
        Plan = string.Empty;
        Preferences = Preferences.Defaults();
        NotificationSettings = NotificationSettings.Defaults();
    }

    public Profile(int userId, string firstName, string lastName, string role, string plan, string? phone,
        string? timezone)
    {
        UserId = userId;
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        AvatarUrl = null;
        Role = role;
        Plan = plan;
        var defaults = Preferences.Defaults();
        Preferences = string.IsNullOrWhiteSpace(timezone) ? defaults : defaults with { Timezone = timezone };
        NotificationSettings = NotificationSettings.Defaults();
    }

    public int Id { get; }
    public int UserId { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public string? Phone { get; private set; }
    public string? AvatarUrl { get; private set; }
    public string Role { get; private set; }
    public string Plan { get; private set; }
    public Preferences Preferences { get; private set; }
    public NotificationSettings NotificationSettings { get; private set; }

    /// <summary>
    ///     Updates the editable profile fields.
    /// </summary>
    public Profile UpdateProfile(string firstName, string lastName, string? phone, string? avatarUrl)
    {
        FirstName = firstName;
        LastName = lastName;
        Phone = phone;
        AvatarUrl = avatarUrl;
        return this;
    }

    /// <summary>
    ///     Replaces the user's preferences.
    /// </summary>
    public Profile UpdatePreferences(Preferences preferences)
    {
        Preferences = preferences;
        return this;
    }

    /// <summary>
    ///     Replaces the user's notification settings.
    /// </summary>
    public Profile UpdateNotificationSettings(NotificationSettings notificationSettings)
    {
        NotificationSettings = notificationSettings;
        return this;
    }
}
