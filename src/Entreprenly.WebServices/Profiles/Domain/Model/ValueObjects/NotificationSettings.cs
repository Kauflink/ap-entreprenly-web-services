namespace Entreprenly.WebServices.Profiles.Domain.Model.ValueObjects;

/// <summary>
///     Notification settings value object.
/// </summary>
/// <param name="StockAlerts">Whether stock alerts are enabled.</param>
/// <param name="PaymentAlerts">Whether payment alerts are enabled.</param>
/// <param name="ChatbotMessages">Whether chatbot message notifications are enabled.</param>
public record NotificationSettings(bool StockAlerts, bool PaymentAlerts, bool ChatbotMessages)
{
    public NotificationSettings() : this(true, false, true)
    {
    }

    /// <summary>
    ///     Returns the default notification settings applied to a freshly created profile.
    ///     The chatbot auto-reply is enabled by default so newly onboarded accounts keep
    ///     responding to their customers until they opt out.
    /// </summary>
    public static NotificationSettings Defaults()
    {
        return new NotificationSettings(true, false, true);
    }
}
