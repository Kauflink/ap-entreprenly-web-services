namespace Entreprenly.WebServices.Inventory.Domain.Model.ValueObjects;

/// <summary>
///     Severity assigned to a stock alert.
/// </summary>
public enum AlertSeverity
{
    Warning,
    Critical
}

public static class AlertSeverityExtensions
{
    /// <summary>
    ///     Returns the lowercase wire value of this severity.
    /// </summary>
    public static string ToValue(this AlertSeverity severity)
    {
        return severity switch
        {
            AlertSeverity.Warning => "warning",
            AlertSeverity.Critical => "critical",
            _ => severity.ToString().ToLowerInvariant()
        };
    }
}
