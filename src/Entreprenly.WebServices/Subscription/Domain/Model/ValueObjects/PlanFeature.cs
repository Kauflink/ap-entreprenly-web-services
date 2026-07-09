namespace Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

public class PlanFeature
{
    public PlanFeature()
    {
        Description = string.Empty;
    }

    public PlanFeature(string description, bool available)
    {
        Description = description;
        Available = available;
    }

    public string Description { get; private set; }
    public bool Available { get; private set; }
}
