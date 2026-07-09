using System.Globalization;
using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Iam.Interfaces.Acl;
using Entreprenly.WebServices.Profiles.Application.QueryServices;
using Entreprenly.WebServices.Profiles.Domain.Model.Queries;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Controllers;

/// <summary>
///     Receives webhook calls FROM the Node.js bridge.
///     These endpoints are called by the bridge, not by the frontend.
///     Protected with X-Bridge-Token to avoid unauthorized calls.
/// </summary>
[ApiController]
[Route("api/v1/chatbot/whatsapp")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("WhatsApp bridge webhook endpoints (called by bridge)")]
public class WhatsappWebhookController(
    IChatbotConversationService chatbotConversationService,
    IIamContextFacade iamFacade,
    IProfileQueryService profileQueryService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory,
    ILogger<WhatsappWebhookController> logger)
    : ControllerBase
{
    [HttpPost("webhook")]
    [AllowAnonymous]
    [SwaggerOperation("Handle inbound text message from bridge",
        "Processes an inbound text message relayed by the WhatsApp bridge and returns the bot reply.",
        OperationId = "HandleInboundMessage")]
    [SwaggerResponse(StatusCodes.Status200OK, "Reply generated")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Session not found")]
    public async Task<IActionResult> HandleMessage([FromBody] InboundMessageResource resource,
        CancellationToken cancellationToken)
    {
        await ApplyOwnerCultureAsync(resource.OwnerEmail, cancellationToken);
        var command = new HandleInboundMessageCommand(resource.FromPhone, resource.ClientName,
            resource.Content, resource.OwnerEmail);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            reply => Ok(new { content = reply }));
    }

    [HttpPost("webhook/receipt")]
    [AllowAnonymous]
    [SwaggerOperation("Handle inbound payment receipt from bridge",
        "Processes an inbound payment receipt image relayed by the WhatsApp bridge.",
        OperationId = "HandleInboundReceipt")]
    [SwaggerResponse(StatusCodes.Status200OK, "Receipt processed")]
    public async Task<IActionResult> HandleReceipt([FromBody] InboundReceiptResource resource,
        CancellationToken cancellationToken)
    {
        await ApplyOwnerCultureAsync(resource.OwnerEmail, cancellationToken);
        var command = new HandleInboundReceiptCommand(resource.FromPhone, resource.OwnerEmail, resource.Image);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            reply => Ok(new { content = reply }));
    }

    [HttpPost("bridge/status")]
    [AllowAnonymous]
    [SwaggerOperation("Bridge reports connection status change",
        "Updates the stored session when the WhatsApp bridge reports a connection status change.",
        OperationId = "ReportBridgeStatus")]
    [SwaggerResponse(StatusCodes.Status200OK, "Status updated", typeof(WhatsappSessionResource))]
    public async Task<IActionResult> ReportStatus([FromBody] BridgeStatusResource resource,
        CancellationToken cancellationToken)
    {
        if (resource.Connected) WhatsappQrStore.Clear(resource.OwnerEmail);
        var command = new ReportBridgeConnectionCommand(resource.Connected, resource.Phone,
            resource.OwnerEmail, resource.BusinessName, resource.SellerId);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            session => Ok(WhatsappSessionResourceFromEntityAssembler.ToResourceFromEntity(session)));
    }

    [HttpPost("bridge/qr")]
    [AllowAnonymous]
    [SwaggerOperation("Bridge reports new QR code", "Stores the latest login QR code reported by the WhatsApp bridge.",
        OperationId = "ReportBridgeQr")]
    [SwaggerResponse(StatusCodes.Status200OK, "QR acknowledged")]
    public IActionResult ReportQr([FromBody] BridgeQrResource resource)
    {
        WhatsappQrStore.Set(resource.OwnerEmail, resource.Qr);
        return Ok(new { received = true, ownerEmail = resource.OwnerEmail });
    }

    [HttpGet("qr")]
    [AllowAnonymous]
    [SwaggerOperation("Get current QR code for frontend polling",
        "Returns the current WhatsApp login QR code for the frontend to poll.",
        OperationId = "GetCurrentQr")]
    [SwaggerResponse(StatusCodes.Status200OK, "QR data URL or null")]
    public IActionResult GetQr([FromQuery] string? ownerEmail)
    {
        var qr = string.IsNullOrEmpty(ownerEmail) ? null : WhatsappQrStore.Get(ownerEmail);
        return Ok(new { qr });
    }

    private async Task ApplyOwnerCultureAsync(string ownerEmail, CancellationToken ct)
    {
        var language = "es";
        var userId = await iamFacade.FetchUserIdByEmail(ownerEmail, ct);
        logger.LogInformation("[Culture] ownerEmail={Email} userId={UserId}", ownerEmail, userId);
        if (userId != 0)
        {
            var profile = await profileQueryService.Handle(new GetProfileByUserIdQuery(userId), ct);
            language = profile?.Preferences?.Language ?? "es";
            logger.LogInformation("[Culture] profileId={ProfileId} language={Language}",
                profile?.Id, language);
        }

        var culture = new CultureInfo(language);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        logger.LogInformation("[Culture] applied culture={Culture}", culture.Name);
    }
}
