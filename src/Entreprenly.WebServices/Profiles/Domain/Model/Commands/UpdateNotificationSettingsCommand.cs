namespace Entreprenly.WebServices.Profiles.Domain.Model.Commands;

/// <summary>
///     Command to replace a profile's notification settings.
/// </summary>
public record UpdateNotificationSettingsCommand(
    int ProfileId,
    bool StockAlerts);
