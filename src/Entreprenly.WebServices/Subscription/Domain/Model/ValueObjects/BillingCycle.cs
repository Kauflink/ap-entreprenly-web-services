namespace Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

public static class BillingCycle
{
    public const string Monthly = "monthly";
    public const string Annual = "annual";

    public static bool IsSupported(string value)
    {
        return value is Monthly or Annual;
    }
}
