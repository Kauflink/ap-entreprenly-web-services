using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Entreprenly.WebServices.Subscription.Domain.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

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
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Domain.Model.Aggregates.Subscription, IActionResult> successAction)
    {
        if (result.IsSuccess) return successAction(result.Value!);

        var statusCode = ToStatusCodeFromSubscriptionError((SubscriptionError)result.Error!);
        return problemDetailsFactory.CreateProblemDetails(controller, statusCode, result.Error, result.Message);
    }

    public static IActionResult ToActionResultFromGetResult(
        ControllerBase controller,
        Domain.Model.Aggregates.Subscription? subscription,
        IStringLocalizer<ErrorMessages> errorLocalizer,
        ProblemDetailsFactory problemDetailsFactory,
        Func<Domain.Model.Aggregates.Subscription, IActionResult> successAction)
    {
        if (subscription is null)
            return problemDetailsFactory.CreateProblemDetails(
                controller,
                ToStatusCodeFromSubscriptionError(SubscriptionError.SubscriptionNotFound),
                SubscriptionError.SubscriptionNotFound,
                errorLocalizer[nameof(SubscriptionError.SubscriptionNotFound)]);

        return successAction(subscription);
    }
}
