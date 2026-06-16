using Entreprenly.WebServices.Shared.Domain.Model.Entities;
using Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Subscription.Domain.Model.Aggregates;

public partial class Subscription : IAuditableEntity
{
    private readonly List<SubscriptionLimit> _limits = [];
    private readonly List<SubscriptionActivity> _activity = [];

    public Subscription()
    {
        DefaultBillingCycle = BillingCycle.Monthly;
        CurrentPlan = SubscriptionPlan.Free();
        RecommendedPlan = SubscriptionPlan.ControlRecommended();
        BillingSetup = BillingSetup.Empty();
    }

    public Subscription(int userId)
    {
        UserId = userId;
        DefaultBillingCycle = BillingCycle.Monthly;
        CurrentPlan = SubscriptionPlan.Free();
        RecommendedPlan = SubscriptionPlan.ControlRecommended();
        BillingSetup = BillingSetup.Empty();
        _limits =
        [
            new SubscriptionLimit("products", "Productos", 0, 10),
            new SubscriptionLimit("active-batches", "Lotes activos", 0, 10),
            new SubscriptionLimit("users", "Usuarios", 1, 1)
        ];
        _activity =
        [
            new SubscriptionActivity("created-account", "Cuenta creada", "Plan Free asignado automaticamente"),
            new SubscriptionActivity("current-status", "Estado actual", "Plan Free activo - Sin cargos registrados"),
            new SubscriptionActivity("billing", "Facturacion", "Sin cargos registrados")
        ];
    }

    public int Id { get; }
    public int UserId { get; private set; }
    public string DefaultBillingCycle { get; private set; }
    public SubscriptionPlan CurrentPlan { get; private set; }
    public SubscriptionPlan RecommendedPlan { get; private set; }
    public IReadOnlyCollection<SubscriptionLimit> Limits => _limits.AsReadOnly();
    public BillingSetup BillingSetup { get; private set; }
    public IReadOnlyCollection<SubscriptionActivity> Activity => _activity.AsReadOnly();
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }

    public Subscription ReplaceDashboard(string defaultBillingCycle, SubscriptionPlan currentPlan,
        SubscriptionPlan recommendedPlan, IEnumerable<SubscriptionLimit> limits, BillingSetup billingSetup,
        IEnumerable<SubscriptionActivity> activity)
    {
        DefaultBillingCycle = BillingCycle.IsSupported(defaultBillingCycle) ? defaultBillingCycle : BillingCycle.Monthly;
        CurrentPlan = currentPlan;
        RecommendedPlan = recommendedPlan;
        BillingSetup = billingSetup;
        ReplaceLimits(limits);
        ReplaceActivity(activity);
        return this;
    }

    public Subscription ActivateControlPlan(string billingCycle, DateOnly startDate)
    {
        DefaultBillingCycle = BillingCycle.IsSupported(billingCycle) ? billingCycle : BillingCycle.Monthly;
        CurrentPlan = SubscriptionPlan.ControlActive(DefaultBillingCycle, startDate);
        RecommendedPlan = SubscriptionPlan.ControlRecommended();
        ReplaceLimits(
        [
            new SubscriptionLimit("products", "Productos", 0, 0),
            new SubscriptionLimit("active-batches", "Lotes activos", 0, 0),
            new SubscriptionLimit("users", "Usuarios", 1, 5)
        ]);
        ReplaceActivity(BuildActiveActivity(CurrentPlan.CurrentPeriodEndDate, DefaultBillingCycle));
        return this;
    }

    public Subscription ScheduleCancellation()
    {
        CurrentPlan = CurrentPlan.ScheduleCancellation();
        ReplaceActivity(BuildScheduledCancellationActivity(CurrentPlan.CurrentPeriodEndDate));
        return this;
    }

    public Subscription KeepControlPlan()
    {
        CurrentPlan = CurrentPlan.KeepRenewal();
        ReplaceActivity(BuildActiveActivity(CurrentPlan.CurrentPeriodEndDate, DefaultBillingCycle));
        return this;
    }

    public Subscription UpdateBillingSetup(BillingSetup billingSetup)
    {
        BillingSetup = billingSetup;
        return this;
    }

    private void ReplaceLimits(IEnumerable<SubscriptionLimit> limits)
    {
        _limits.Clear();
        _limits.AddRange(limits);
    }

    private void ReplaceActivity(IEnumerable<SubscriptionActivity> activity)
    {
        _activity.Clear();
        _activity.AddRange(activity);
    }

    private static List<SubscriptionActivity> BuildActiveActivity(DateOnly? endDate, string billingCycle)
    {
        var endDateText = endDate is null
            ? "la fecha registrada en tu suscripcion"
            : SubscriptionPlan.FormatDate(endDate.Value);
        return
        [
            new SubscriptionActivity("created-account", "Cuenta creada", "Plan Free asignado automaticamente"),
            new SubscriptionActivity("current-status", "Estado actual", "Plan Control activo"),
            new SubscriptionActivity("billing", "Facturacion",
                $"Proxima renovacion: {endDateText} - {BillingCycleLabel(billingCycle)}")
        ];
    }

    private static List<SubscriptionActivity> BuildScheduledCancellationActivity(DateOnly? endDate)
    {
        var endDateText = endDate is null
            ? "la fecha registrada en tu suscripcion"
            : SubscriptionPlan.FormatDate(endDate.Value);
        return
        [
            new SubscriptionActivity("created-account", "Cuenta creada", "Plan Free asignado automaticamente"),
            new SubscriptionActivity("current-status", "Estado actual", "Cancelacion programada"),
            new SubscriptionActivity("billing", "Facturacion", $"Acceso vigente hasta el {endDateText} - sin siguiente cobro")
        ];
    }

    private static string BillingCycleLabel(string billingCycle)
    {
        return billingCycle == BillingCycle.Annual ? "pago anual" : "pago mensual";
    }
}
