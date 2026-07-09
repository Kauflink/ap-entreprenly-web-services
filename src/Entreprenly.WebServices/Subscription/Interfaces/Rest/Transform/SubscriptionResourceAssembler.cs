using Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Subscription.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Subscription.Interfaces.Rest.Transform;

public static class SubscriptionResourceAssembler
{
    public static SubscriptionDashboardResource ToResourceFromEntity(Domain.Model.Aggregates.Subscription subscription)
    {
        return new SubscriptionDashboardResource(
            subscription.Id,
            subscription.DefaultBillingCycle,
            ToResourceFromPlan(subscription.CurrentPlan),
            ToResourceFromPlan(subscription.RecommendedPlan),
            subscription.Limits.Select(ToResourceFromLimit),
            ToResourceFromBillingSetup(subscription.BillingSetup),
            subscription.Activity.Select(ToResourceFromActivity));
    }

    public static SubscriptionPlan ToPlanFromResource(SubscriptionPlanResource resource)
    {
        return new SubscriptionPlan(
            resource.Id,
            resource.Name,
            resource.ShortDescription,
            resource.MonthlyPrice,
            resource.AnnualPrice,
            resource.Status,
            resource.StatusLabel,
            resource.BadgeLabel,
            resource.Recommended,
            ParseDate(resource.CurrentPeriodStartDate),
            ParseDate(resource.CurrentPeriodEndDate),
            resource.Features.Select(feature => new PlanFeature(feature.Description, feature.Available)));
    }

    public static SubscriptionLimit ToLimitFromResource(SubscriptionLimitResource resource)
    {
        return new SubscriptionLimit(resource.Id, resource.Label, resource.UsedValue, resource.MaxValue);
    }

    public static SubscriptionActivity ToActivityFromResource(SubscriptionActivityResource resource)
    {
        return new SubscriptionActivity(resource.Id, resource.Title, resource.Detail);
    }

    public static BillingSetup ToBillingSetupFromResource(BillingSetupResource resource)
    {
        return new BillingSetup(
            resource.PaymentMethodTitle,
            resource.PaymentMethodDescription,
            resource.PaymentMethodActionLabel,
            resource.FiscalDataTitle,
            resource.FiscalDataDescription,
            resource.FiscalDataActionLabel,
            resource.HasPaymentMethod,
            resource.HasFiscalData,
            BillingSetup.NormalizePaymentMethods(resource.PaymentMethods.Select(paymentMethod => new PaymentMethod(
                paymentMethod.Id,
                paymentMethod.CardBrand,
                paymentMethod.LastFour,
                paymentMethod.HolderName,
                paymentMethod.ExpiryMonth,
                paymentMethod.ExpiryYear,
                paymentMethod.IsDefault))),
            resource.FiscalData is null
                ? null
                : new FiscalData(resource.FiscalData.DocumentType, resource.FiscalData.DocumentNumber,
                    resource.FiscalData.BusinessName, resource.FiscalData.ReceiptEmail,
                    resource.FiscalData.FiscalAddress));
    }

    private static SubscriptionPlanResource ToResourceFromPlan(SubscriptionPlan plan)
    {
        return new SubscriptionPlanResource(
            plan.PlanId,
            plan.Name,
            plan.ShortDescription,
            plan.MonthlyPrice,
            plan.AnnualPrice,
            plan.Status,
            plan.StatusLabel,
            plan.BadgeLabel,
            plan.Recommended,
            plan.Features.Select(feature => new PlanFeatureResource(feature.Description, feature.Available)),
            plan.CurrentPeriodStartDate?.ToString("yyyy-MM-dd"),
            plan.CurrentPeriodEndDate?.ToString("yyyy-MM-dd"));
    }

    private static SubscriptionLimitResource ToResourceFromLimit(SubscriptionLimit limit)
    {
        return new SubscriptionLimitResource(limit.LimitId, limit.Label, limit.UsedValue, limit.MaxValue);
    }

    private static SubscriptionActivityResource ToResourceFromActivity(SubscriptionActivity activity)
    {
        return new SubscriptionActivityResource(activity.ActivityId, activity.Title, activity.Detail);
    }

    private static BillingSetupResource ToResourceFromBillingSetup(BillingSetup billingSetup)
    {
        return new BillingSetupResource(
            billingSetup.PaymentMethodTitle,
            billingSetup.PaymentMethodDescription,
            billingSetup.PaymentMethodActionLabel,
            billingSetup.FiscalDataTitle,
            billingSetup.FiscalDataDescription,
            billingSetup.FiscalDataActionLabel,
            billingSetup.HasPaymentMethod,
            billingSetup.HasFiscalData,
            BillingSetup.NormalizePaymentMethods(billingSetup.PaymentMethods).Select(paymentMethod => new PaymentMethodResource(
                paymentMethod.PaymentMethodId,
                paymentMethod.CardBrand,
                paymentMethod.LastFour,
                paymentMethod.HolderName,
                paymentMethod.ExpiryMonth,
                paymentMethod.ExpiryYear,
                paymentMethod.IsDefault)),
            billingSetup.FiscalData is null
                ? null
                : new FiscalDataResource(
                    billingSetup.FiscalData.DocumentType,
                    billingSetup.FiscalData.DocumentNumber,
                    billingSetup.FiscalData.BusinessName,
                    billingSetup.FiscalData.ReceiptEmail,
                    billingSetup.FiscalData.FiscalAddress));
    }

    private static DateOnly? ParseDate(string? value)
    {
        return DateOnly.TryParse(value, out var date) ? date : null;
    }
}
