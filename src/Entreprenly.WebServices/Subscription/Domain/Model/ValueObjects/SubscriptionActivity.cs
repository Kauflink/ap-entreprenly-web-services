namespace Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;

public class SubscriptionActivity
{
    public SubscriptionActivity()
    {
        ActivityId = string.Empty;
        Title = string.Empty;
        Detail = string.Empty;
    }

    public SubscriptionActivity(string activityId, string title, string detail)
    {
        ActivityId = activityId;
        Title = title;
        Detail = detail;
    }

    public string ActivityId { get; private set; }
    public string Title { get; private set; }
    public string Detail { get; private set; }
}
