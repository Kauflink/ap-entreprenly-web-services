using Entreprenly.WebServices.Shared.Domain.Model;

namespace Entreprenly.WebServices.Subscription.Domain.Model.Errors;

/// <summary>
///     Catalog of Subscription domain errors, exposed as reusable <see cref="Error" /> instances.
/// </summary>
public static class SubscriptionErrors
{
    public static readonly Error SubscriptionNotFound =
        new("Subscription.SubscriptionNotFound", "Subscription dashboard not found.");

    public static readonly Error SubscriptionAlreadyExists =
        new("Subscription.SubscriptionAlreadyExists", "A subscription dashboard already exists for this user.");

    public static readonly Error InvalidBillingCycle =
        new("Subscription.InvalidBillingCycle", "The billing cycle is not supported.");

    public static readonly Error InvalidSubscriptionData =
        new("Subscription.InvalidSubscriptionData", "Invalid subscription data provided.");

    public static readonly Error OperationCancelled =
        new("Subscription.OperationCancelled", "The subscription operation was cancelled.");

    public static readonly Error DatabaseError =
        new("Subscription.DatabaseError", "A database error occurred while processing the subscription.");

    public static readonly Error InternalServerError =
        new("Subscription.InternalServerError", "An unexpected error occurred while processing the subscription.");

    public static Error From(SubscriptionError error)
    {
        return error switch
        {
            SubscriptionError.SubscriptionNotFound => SubscriptionNotFound,
            SubscriptionError.SubscriptionAlreadyExists => SubscriptionAlreadyExists,
            SubscriptionError.InvalidBillingCycle => InvalidBillingCycle,
            SubscriptionError.InvalidSubscriptionData => InvalidSubscriptionData,
            SubscriptionError.OperationCancelled => OperationCancelled,
            SubscriptionError.DatabaseError => DatabaseError,
            SubscriptionError.InternalServerError => InternalServerError,
            _ => Error.None
        };
    }
}
