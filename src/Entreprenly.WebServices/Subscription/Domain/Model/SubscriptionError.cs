namespace Entreprenly.WebServices.Subscription.Domain.Model;

public enum SubscriptionError
{
    None,
    SubscriptionNotFound,
    SubscriptionAlreadyExists,
    InvalidBillingCycle,
    InvalidSubscriptionData,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
