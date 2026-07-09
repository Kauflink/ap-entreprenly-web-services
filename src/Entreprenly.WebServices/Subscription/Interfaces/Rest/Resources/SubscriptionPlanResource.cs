namespace Entreprenly.WebServices.Subscription.Interfaces.Rest.Resources;

public record SubscriptionPlanResource(
    string Id,
    string Name,
    string ShortDescription,
    decimal MonthlyPrice,
    decimal AnnualPrice,
    string Status,
    string StatusLabel,
    string BadgeLabel,
    bool Recommended,
    IEnumerable<PlanFeatureResource> Features,
    string? CurrentPeriodStartDate = null,
    string? CurrentPeriodEndDate = null);
