using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Entreprenly.WebServices.Subscription.Domain.Model;
using Entreprenly.WebServices.Subscription.Domain.Model.Errors;
using Microsoft.AspNetCore.Mvc;

namespace Entreprenly.WebServices.Subscription.Interfaces.Rest.Transform;

public static class SubscriptionActionResultAssembler
{
    private static int ToStatusCodeFromSubscriptionError(SubscriptionError error)
    {
        return error switch
        {
            SubscriptionError.SubscriptionNotFound => StatusCodes.Status404NotFound,
            SubscriptionError.SubscriptionAlreadyExists => StatusCodes.Status409Conflict,
            SubscriptionError.InvalidBillingCycle => StatusCodes.Status400BadRequest,
            SubscriptionError.InvalidSubscriptionData => StatusCodes.Status400BadRequest,
            SubscriptionError.OperationCancelled => StatusCodes.Status409Conflict,
            SubscriptionError.DatabaseError => StatusCodes.Status500InternalServerError,
            SubscriptionError.InternalServerError => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };
    }

    public static IActionResult ToActionResultFromResult(
        ControllerBase controller,
        Result<Domain.Model.Aggregates.Subscription> result,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Domain.Model.Aggregates.Subscription, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromSubscriptionError((SubscriptionError)result.Error!);
        var error = SubscriptionErrors.From((SubscriptionError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, error.Message, result.Message);
    }

    public static IActionResult ToActionResultFromGetResult(
        ControllerBase controller,
        Domain.Model.Aggregates.Subscription? subscription,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Domain.Model.Aggregates.Subscription, IActionResult> successAction)
    {
        if (subscription is null)
            return problemDetailsFactory.CreateProblemDetails(
                controller,
                ToStatusCodeFromSubscriptionError(SubscriptionError.SubscriptionNotFound),
                SubscriptionErrors.SubscriptionNotFound.Message,
                SubscriptionErrors.SubscriptionNotFound.Message);

        return successAction(subscription);
    }
}
