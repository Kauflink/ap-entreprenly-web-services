using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
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
public class WhatsappWebhookController(IChatbotConversationService chatbotConversationService) : ControllerBase
{
    [HttpPost("webhook")]
    [AllowAnonymous]
    [SwaggerOperation("Handle inbound text message from bridge", OperationId = "HandleInboundMessage")]
    [SwaggerResponse(StatusCodes.Status200OK, "Reply generated")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Session not found")]
    public async Task<IActionResult> HandleMessage([FromBody] InboundMessageResource resource,
        CancellationToken cancellationToken)
    {
        var command = new HandleInboundMessageCommand(resource.FromPhone, resource.ClientName,
            resource.Content, resource.OwnerEmail);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        if (!result.IsSuccess) return BadRequest(new { error = result.Message });
        return Ok(new { content = result.Value });
    }

    [HttpPost("webhook/receipt")]
    [AllowAnonymous]
    [SwaggerOperation("Handle inbound payment receipt from bridge", OperationId = "HandleInboundReceipt")]
    [SwaggerResponse(StatusCodes.Status200OK, "Receipt processed")]
    public async Task<IActionResult> HandleReceipt([FromBody] InboundReceiptResource resource,
        CancellationToken cancellationToken)
    {
        var command = new HandleInboundReceiptCommand(resource.FromPhone, resource.OwnerEmail, resource.Image);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        if (!result.IsSuccess) return BadRequest(new { error = result.Message });
        return Ok(new { content = result.Value });
    }

    [HttpPost("bridge/status")]
    [AllowAnonymous]
    [SwaggerOperation("Bridge reports connection status change", OperationId = "ReportBridgeStatus")]
    [SwaggerResponse(StatusCodes.Status200OK, "Status updated", typeof(WhatsappSessionResource))]
    public async Task<IActionResult> ReportStatus([FromBody] BridgeStatusResource resource,
        CancellationToken cancellationToken)
    {
        var command = new ReportBridgeConnectionCommand(resource.Connected, resource.Phone,
            resource.OwnerEmail, resource.BusinessName, resource.SellerId);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        if (!result.IsSuccess) return BadRequest(new { error = result.Message });
        return Ok(WhatsappSessionResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }

    [HttpPost("bridge/qr")]
    [AllowAnonymous]
    [SwaggerOperation("Bridge reports new QR code", OperationId = "ReportBridgeQr")]
    [SwaggerResponse(StatusCodes.Status200OK, "QR acknowledged")]
    public IActionResult ReportQr([FromBody] BridgeQrResource resource)
    {
        return Ok(new { received = true, ownerEmail = resource.OwnerEmail });
    }
}
