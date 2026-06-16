using Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

namespace Entreprenly.WebServices.Subscription.Domain.Model.Commands;

public record UpdateBillingSetupCommand(int UserId, BillingSetup BillingSetup);
