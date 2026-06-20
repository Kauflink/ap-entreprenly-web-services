namespace Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

public class SubscriptionPlan
{
    private readonly List<PlanFeature> _features = [];

    public SubscriptionPlan()
    {
        PlanId = string.Empty;
        Name = string.Empty;
        ShortDescription = string.Empty;
        Status = string.Empty;
        StatusLabel = string.Empty;
        BadgeLabel = string.Empty;
    }

    public SubscriptionPlan(
        string planId,
        string name,
        string shortDescription,
        decimal monthlyPrice,
        decimal annualPrice,
        string status,
        string statusLabel,
        string badgeLabel,
        bool recommended,
        DateOnly? currentPeriodStartDate,
        DateOnly? currentPeriodEndDate,
        IEnumerable<PlanFeature> features)
    {
        PlanId = planId;
        Name = name;
        ShortDescription = shortDescription;
        MonthlyPrice = monthlyPrice;
        AnnualPrice = annualPrice;
        Status = status;
        StatusLabel = statusLabel;
        BadgeLabel = badgeLabel;
        Recommended = recommended;
        CurrentPeriodStartDate = currentPeriodStartDate;
        CurrentPeriodEndDate = currentPeriodEndDate;
        _features = features.ToList();
    }

    public string PlanId { get; private set; }
    public string Name { get; private set; }
    public string ShortDescription { get; private set; }
    public decimal MonthlyPrice { get; private set; }
    public decimal AnnualPrice { get; private set; }
    public string Status { get; private set; }
    public string StatusLabel { get; private set; }
    public string BadgeLabel { get; private set; }
    public bool Recommended { get; private set; }
    public DateOnly? CurrentPeriodStartDate { get; private set; }
    public DateOnly? CurrentPeriodEndDate { get; private set; }
    public IReadOnlyCollection<PlanFeature> Features => _features.AsReadOnly();

    public static SubscriptionPlan Free()
    {
        return new SubscriptionPlan(
            "plan-free",
            "Plan Free",
            "Empieza con limites basicos para validar tu operacion.",
            0,
            0,
            PlanStatus.Free,
            "Plan Free activo",
            "Plan actual",
            false,
            null,
            null,
            [
                new PlanFeature("Hasta 10 productos y 10 lotes activos.", true),
                new PlanFeature("Ventas y caja con funciones basicas.", true),
                new PlanFeature("Chatbot de WhatsApp no incluido.", false)
            ]);
    }

    public static SubscriptionPlan ControlRecommended()
    {
        return new SubscriptionPlan(
            "plan-control",
            "Plan Control",
            "Opera sin restricciones con automatizaciones, alertas y trazabilidad completa.",
            89,
            890,
            PlanStatus.Active,
            "Recomendado",
            "Recomendado",
            true,
            null,
            null,
            ControlFeatures());
    }

    public static SubscriptionPlan ControlActive(string billingCycle, DateOnly startDate)
    {
        var endDate = AddBillingCycle(startDate, billingCycle);
        return new SubscriptionPlan(
            "plan-control",
            "Plan Control",
            $"Tu plan sigue activo hasta el {FormatDate(endDate)}. Se renovara automaticamente.",
            89,
            890,
            PlanStatus.Active,
            "Plan Control activo",
            "Plan actual",
            false,
            startDate,
            endDate,
            ControlFeatures());
    }

    public SubscriptionPlan ScheduleCancellation()
    {
        var endDateText = CurrentPeriodEndDate is null
            ? "la fecha registrada en tu suscripcion"
            : FormatDate(CurrentPeriodEndDate.Value);

        return new SubscriptionPlan(
            PlanId,
            Name,
            $"Tu plan sigue activo hasta el {endDateText}. No se renovara automaticamente.",
            MonthlyPrice,
            AnnualPrice,
            PlanStatus.ScheduledCancellation,
            "Cancelacion programada",
            BadgeLabel,
            Recommended,
            CurrentPeriodStartDate,
            CurrentPeriodEndDate,
            CloneFeatures());
    }

    public SubscriptionPlan KeepRenewal()
    {
        var endDateText = CurrentPeriodEndDate is null
            ? "la fecha registrada en tu suscripcion"
            : FormatDate(CurrentPeriodEndDate.Value);

        return new SubscriptionPlan(
            PlanId,
            Name,
            $"Tu plan sigue activo hasta el {endDateText}. Se renovara automaticamente.",
            MonthlyPrice,
            AnnualPrice,
            PlanStatus.Active,
            "Plan Control activo",
            BadgeLabel,
            Recommended,
            CurrentPeriodStartDate,
            CurrentPeriodEndDate,
            CloneFeatures());
    }

    // Rebuild the owned PlanFeature instances so EF Core tracks them as new children of the
    // replaced CurrentPlan instead of re-parenting the ones it already tracks (which throws).
    private List<PlanFeature> CloneFeatures()
    {
        return _features.Select(feature => new PlanFeature(feature.Description, feature.Available)).ToList();
    }

    private static List<PlanFeature> ControlFeatures()
    {
        return
        [
            new PlanFeature("Productos y lotes ilimitados", true),
            new PlanFeature("Ventas, pedidos, caja y trazabilidad en un solo flujo.", true),
            new PlanFeature("Chatbot de WhatsApp y alertas operativas incluidas.", true)
        ];
    }

    private static DateOnly AddBillingCycle(DateOnly date, string billingCycle)
    {
        return billingCycle == BillingCycle.Annual ? date.AddMonths(12) : date.AddMonths(1);
    }

    public static string FormatDate(DateOnly date)
    {
        var month = date.Month switch
        {
            1 => "enero",
            2 => "febrero",
            3 => "marzo",
            4 => "abril",
            5 => "mayo",
            6 => "junio",
            7 => "julio",
            8 => "agosto",
            9 => "septiembre",
            10 => "octubre",
            11 => "noviembre",
            12 => "diciembre",
            _ => string.Empty
        };
        return $"{date.Day} de {month} de {date.Year}";
    }
}
