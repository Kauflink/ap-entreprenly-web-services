namespace Entreprenly.WebServices.Profiles.Domain.Model.ValueObjects;

/// <summary>
///     User preferences value object.
/// </summary>
/// <param name="Language">UI language code (e.g. en, es).</param>
/// <param name="Timezone">Display timezone.</param>
/// <param name="Theme">UI theme (e.g. light, dark).</param>
/// <param name="Currency">Preferred currency code (e.g. USD, PEN).</param>
public record Preferences(string Language, string Timezone, string Theme, string Currency)
{
    public Preferences() : this("en", "UTC", "light", "USD")
    {
    }

    /// <summary>
    ///     Returns the default preferences applied to a freshly created profile.
    /// </summary>
    public static Preferences Defaults()
    {
        return new Preferences("en", "UTC", "light", "USD");
    }
}
