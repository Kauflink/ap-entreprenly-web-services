namespace Entreprenly.WebServices.Profiles.Domain.Model.ValueObjects;

/// <summary>
///     Notification settings value object.
/// </summary>
/// <param name="StockAlerts">Whether stock alerts are enabled.</param>
public record NotificationSettings(bool StockAlerts)
{
    public NotificationSettings() : this(true)
    {
    }

    /// <summary>
    ///     Returns the default notification settings applied to a freshly created profile.
    /// </summary>
    public static NotificationSettings Defaults()
    {
        return new NotificationSettings(true);
    }
}
