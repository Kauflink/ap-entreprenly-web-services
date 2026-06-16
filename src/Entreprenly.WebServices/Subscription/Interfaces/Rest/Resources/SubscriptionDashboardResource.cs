namespace Entreprenly.WebServices.Subscription.Interfaces.Rest.Resources;

public record SubscriptionDashboardResource(
    int Id,
    string DefaultBillingCycle,
    SubscriptionPlanResource CurrentPlan,
    SubscriptionPlanResource RecommendedPlan,
    IEnumerable<SubscriptionLimitResource> Limits,
    BillingSetupResource BillingSetup,
    IEnumerable<SubscriptionActivityResource> Activity);
