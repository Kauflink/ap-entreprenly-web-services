using System.Net.Mime;
using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Entreprenly.WebServices.Subscription.Application.CommandServices;
using Entreprenly.WebServices.Subscription.Application.QueryServices;
using Entreprenly.WebServices.Subscription.Domain.Model.Commands;
using Entreprenly.WebServices.Subscription.Domain.Model.Errors;
using Entreprenly.WebServices.Subscription.Domain.Model.Queries;
using Entreprenly.WebServices.Subscription.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Subscription.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Subscription.Interfaces.Rest.Transform;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Subscription.Interfaces.Rest;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Available Subscription endpoints")]
public class SubscriptionsController(
    ISubscriptionCommandService subscriptionCommandService,
    ISubscriptionQueryService subscriptionQueryService,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    private int CurrentUserId => (HttpContext.Items["User"] as User)?.Id ?? 0;

    // ── Current-user shortcuts (used by the frontend) ──────────────────────────

    [HttpGet("me/dashboard")]
    [SwaggerOperation("Get my subscription dashboard", "Get the subscription dashboard for the authenticated user.",
        OperationId = "GetMySubscriptionDashboard")]
    [SwaggerResponse(StatusCodes.Status200OK, "The subscription dashboard was found",
        typeof(SubscriptionDashboardResource))]
    public Task<IActionResult> GetMyDashboard(CancellationToken cancellationToken)
        => GetDashboardByUserId(CurrentUserId, cancellationToken);

    [HttpGet("me/payment-confirmation")]
    [SwaggerOperation("Get my payment confirmation template",
        "Get the Control Plan payment confirmation template for the authenticated user.",
        OperationId = "GetMySubscriptionPaymentConfirmation")]
    [SwaggerResponse(StatusCodes.Status200OK, "The payment confirmation template was built",
        typeof(SubscriptionDashboardResource))]
    public IActionResult GetMyPaymentConfirmation()
        => GetPaymentConfirmation(CurrentUserId);

    [HttpPut("me/dashboard")]
    [SwaggerOperation("Replace my subscription dashboard",
        "Replace the subscription dashboard for the authenticated user.",
        OperationId = "ReplaceMySubscriptionDashboard")]
    [SwaggerResponse(StatusCodes.Status200OK, "The subscription dashboard was updated",
        typeof(SubscriptionDashboardResource))]
    public Task<IActionResult> ReplaceMyDashboard([FromBody] SubscriptionDashboardResource resource,
        CancellationToken cancellationToken)
        => ReplaceDashboard(CurrentUserId, resource, cancellationToken);

    // ── Existing by-userId endpoints ────────────────────────────────────────────

    [HttpGet("active")]
    [SwaggerOperation("Get active subscription by user id",
        "Get the active non-expired subscription dashboard for a user.",
        OperationId = "GetActiveSubscriptionByUserId")]
    [SwaggerResponse(StatusCodes.Status200OK, "The active subscription was found",
        typeof(SubscriptionDashboardResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The active subscription was not found")]
    public async Task<IActionResult> GetActiveSubscriptionByUserId([FromQuery] int userId,
        CancellationToken cancellationToken)
    {
        var subscription = await subscriptionQueryService.Handle(new GetSubscriptionByUserIdQuery(userId),
            cancellationToken);

        if (subscription is null || !IsActive(subscription))
            return problemDetailsFactory.CreateProblemDetails(
                this,
                StatusCodes.Status404NotFound,
                SubscriptionErrors.SubscriptionNotFound.Message,
                SubscriptionErrors.SubscriptionNotFound.Message);

        return Ok(SubscriptionResourceAssembler.ToResourceFromEntity(subscription));
    }

    [HttpGet("by-user/{userId:int}/dashboard")]
    [SwaggerOperation("Get subscription dashboard", "Get a user's subscription dashboard.",
        OperationId = "GetSubscriptionDashboardByUserId")]
    [SwaggerResponse(StatusCodes.Status200OK, "The subscription dashboard was found",
        typeof(SubscriptionDashboardResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "The subscription dashboard was not found")]
    public async Task<IActionResult> GetDashboardByUserId(int userId, CancellationToken cancellationToken)
    {
        var subscription = await subscriptionQueryService.Handle(new GetSubscriptionByUserIdQuery(userId),
            cancellationToken);
        if (subscription is null)
        {
            var createResult = await subscriptionCommandService.Handle(new CreateSubscriptionCommand(userId),
                cancellationToken);
            return SubscriptionActionResultAssembler.ToActionResultFromResult(
                this, createResult, problemDetailsFactory,
                created => Ok(SubscriptionResourceAssembler.ToResourceFromEntity(created)));
        }

        return SubscriptionActionResultAssembler.ToActionResultFromGetResult(
            this, subscription, problemDetailsFactory,
            found => Ok(SubscriptionResourceAssembler.ToResourceFromEntity(found)));
    }

    [HttpGet("by-user/{userId:int}/payment-confirmation")]
    [SwaggerOperation("Get payment confirmation template", "Get a Control Plan dashboard template for client-side payment confirmation.",
        OperationId = "GetSubscriptionPaymentConfirmation")]
    [SwaggerResponse(StatusCodes.Status200OK, "The payment confirmation template was built",
        typeof(SubscriptionDashboardResource))]
    public IActionResult GetPaymentConfirmation(int userId)
    {
        var subscription = new Domain.Model.Aggregates.Subscription(userId)
            .ActivateControlPlan(BillingCycle.Monthly, DateOnly.FromDateTime(DateTime.UtcNow));

        return Ok(SubscriptionResourceAssembler.ToResourceFromEntity(subscription));
    }

    [HttpPost("by-user/{userId:int}/dashboard")]
    [SwaggerOperation("Create subscription dashboard", "Create a default subscription dashboard for a user.",
        OperationId = "CreateSubscriptionDashboard")]
    [SwaggerResponse(StatusCodes.Status201Created, "The subscription dashboard was created",
        typeof(SubscriptionDashboardResource))]
    [SwaggerResponse(StatusCodes.Status409Conflict, "A subscription dashboard already exists for the user")]
    public async Task<IActionResult> CreateDashboard(int userId, CancellationToken cancellationToken)
    {
        var result = await subscriptionCommandService.Handle(new CreateSubscriptionCommand(userId), cancellationToken);
        return SubscriptionActionResultAssembler.ToActionResultFromResult(
            this, result, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetDashboardByUserId), new { userId = created.UserId },
                SubscriptionResourceAssembler.ToResourceFromEntity(created)));
    }

    [HttpPut("by-user/{userId:int}/dashboard")]
    [SwaggerOperation("Replace subscription dashboard", "Replace the user's subscription dashboard.",
        OperationId = "ReplaceSubscriptionDashboard")]
    [SwaggerResponse(StatusCodes.Status200OK, "The subscription dashboard was updated",
        typeof(SubscriptionDashboardResource))]
    public async Task<IActionResult> ReplaceDashboard(int userId, [FromBody] SubscriptionDashboardResource resource,
        CancellationToken cancellationToken)
    {
        var command = new ReplaceSubscriptionDashboardCommand(
            userId,
            resource.DefaultBillingCycle,
            SubscriptionResourceAssembler.ToPlanFromResource(resource.CurrentPlan),
            SubscriptionResourceAssembler.ToPlanFromResource(resource.RecommendedPlan),
            resource.Limits.Select(SubscriptionResourceAssembler.ToLimitFromResource),
            SubscriptionResourceAssembler.ToBillingSetupFromResource(resource.BillingSetup),
            resource.Activity.Select(SubscriptionResourceAssembler.ToActivityFromResource));
        var result = await subscriptionCommandService.Handle(command, cancellationToken);

        return SubscriptionActionResultAssembler.ToActionResultFromResult(
            this, result, problemDetailsFactory,
            updated => Ok(SubscriptionResourceAssembler.ToResourceFromEntity(updated)));
    }

    [HttpPut("by-user/{userId:int}/billing-setup")]
    [SwaggerOperation("Update billing setup", "Update payment methods and fiscal data.",
        OperationId = "UpdateBillingSetup")]
    [SwaggerResponse(StatusCodes.Status200OK, "The billing setup was updated",
        typeof(SubscriptionDashboardResource))]
    public async Task<IActionResult> UpdateBillingSetup(int userId, [FromBody] BillingSetupResource resource,
        CancellationToken cancellationToken)
    {
        var command = new UpdateBillingSetupCommand(userId,
            SubscriptionResourceAssembler.ToBillingSetupFromResource(resource));
        var result = await subscriptionCommandService.Handle(command, cancellationToken);

        return SubscriptionActionResultAssembler.ToActionResultFromResult(
            this, result, problemDetailsFactory,
            updated => Ok(SubscriptionResourceAssembler.ToResourceFromEntity(updated)));
    }

    [HttpPost("by-user/{userId:int}/activate-control")]
    [SwaggerOperation("Activate Control Plan", "Activate Control Plan for the user.",
        OperationId = "ActivateControlPlan")]
    [SwaggerResponse(StatusCodes.Status200OK, "The Control Plan was activated",
        typeof(SubscriptionDashboardResource))]
    public async Task<IActionResult> ActivateControlPlan(int userId, [FromBody] ActivateControlPlanResource resource,
        CancellationToken cancellationToken)
    {
        var result = await subscriptionCommandService.Handle(new ActivateControlPlanCommand(userId,
            resource.BillingCycle), cancellationToken);
        return SubscriptionActionResultAssembler.ToActionResultFromResult(
            this, result, problemDetailsFactory,
            updated => Ok(SubscriptionResourceAssembler.ToResourceFromEntity(updated)));
    }

    [HttpPost("by-user/{userId:int}/schedule-cancellation")]
    [SwaggerOperation("Schedule cancellation", "Schedule cancellation for the user's Control Plan.",
        OperationId = "ScheduleSubscriptionCancellation")]
    [SwaggerResponse(StatusCodes.Status200OK, "The cancellation was scheduled",
        typeof(SubscriptionDashboardResource))]
    public async Task<IActionResult> ScheduleCancellation(int userId, CancellationToken cancellationToken)
    {
        var result = await subscriptionCommandService.Handle(new ScheduleSubscriptionCancellationCommand(userId),
            cancellationToken);
        return SubscriptionActionResultAssembler.ToActionResultFromResult(
            this, result, problemDetailsFactory,
            updated => Ok(SubscriptionResourceAssembler.ToResourceFromEntity(updated)));
    }

    [HttpPost("by-user/{userId:int}/keep-control")]
    [SwaggerOperation("Keep Control Plan", "Cancel a scheduled cancellation and keep renewal active.",
        OperationId = "KeepControlPlan")]
    [SwaggerResponse(StatusCodes.Status200OK, "The Control Plan will keep renewing",
        typeof(SubscriptionDashboardResource))]
    public async Task<IActionResult> KeepControlPlan(int userId, CancellationToken cancellationToken)
    {
        var result = await subscriptionCommandService.Handle(new KeepControlPlanCommand(userId), cancellationToken);
        return SubscriptionActionResultAssembler.ToActionResultFromResult(
            this, result, problemDetailsFactory,
            updated => Ok(SubscriptionResourceAssembler.ToResourceFromEntity(updated)));
    }

    private static bool IsActive(Domain.Model.Aggregates.Subscription subscription)
    {
        var status = subscription.CurrentPlan.Status;
        var endDate = subscription.CurrentPlan.CurrentPeriodEndDate;
        return status is PlanStatus.Free or PlanStatus.Active or PlanStatus.ScheduledCancellation
               && (endDate is null || endDate >= DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
