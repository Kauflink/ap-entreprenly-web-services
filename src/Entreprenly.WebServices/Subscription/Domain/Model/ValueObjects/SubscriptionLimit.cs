namespace Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

public class SubscriptionLimit
{
    public SubscriptionLimit()
    {
        LimitId = string.Empty;
        Label = string.Empty;
    }

    public SubscriptionLimit(string limitId, string label, int usedValue, int maxValue)
    {
        LimitId = limitId;
        Label = label;
        UsedValue = usedValue;
        MaxValue = maxValue;
    }

    public string LimitId { get; private set; }
    public string Label { get; private set; }
    public int UsedValue { get; private set; }
    public int MaxValue { get; private set; }
}
