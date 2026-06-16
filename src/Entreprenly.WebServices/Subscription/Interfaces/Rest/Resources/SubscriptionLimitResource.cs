namespace Entreprenly.WebServices.Subscription.Interfaces.Rest.Resources;

public record SubscriptionLimitResource(string Id, string Label, int UsedValue, int MaxValue);
