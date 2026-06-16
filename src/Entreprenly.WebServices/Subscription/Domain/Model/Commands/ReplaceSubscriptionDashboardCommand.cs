using Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Subscription.Domain.Model.Commands;

public record ReplaceSubscriptionDashboardCommand(
    int UserId,
    string DefaultBillingCycle,
    SubscriptionPlan CurrentPlan,
    SubscriptionPlan RecommendedPlan,
    IEnumerable<SubscriptionLimit> Limits,
    BillingSetup BillingSetup,
    IEnumerable<SubscriptionActivity> Activity);
