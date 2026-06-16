namespace Entreprenly.WebServices.Subscription.Domain.Model.Commands;

public record ActivateControlPlanCommand(int UserId, string BillingCycle);
