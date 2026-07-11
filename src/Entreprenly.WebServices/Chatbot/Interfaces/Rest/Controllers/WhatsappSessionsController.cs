using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.Internal.OutboundServices;
using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Model;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Iam.Interfaces.Acl;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("WhatsApp session endpoints")]
public class WhatsappSessionsController(
    IWhatsappSessionQueryService whatsappSessionQueryService,
    IChatbotConversationService chatbotConversationService,
    IWhatsAppMessagingService messagingService,
    IIamContextFacade iamFacade,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    private const string DefaultBusinessName = "Mi Negocio";

    [HttpGet]
    [SwaggerOperation("Get all WhatsApp sessions",
        "Returns the WhatsApp bridge session(s) belonging to the authenticated account.",
        OperationId = "GetAllWhatsappSessions")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of sessions", typeof(IEnumerable<WhatsappSessionResource>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var sellerId = await CurrentSellerIdAsync(cancellationToken);
        var sessions = await whatsappSessionQueryService.Handle(new GetAllWhatsappSessionsQuery(sellerId), cancellationToken);
        return Ok(sessions.Select(WhatsappSessionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [SwaggerOperation("Create a WhatsApp session", "Registers a new WhatsApp bridge session for an owner.",
        OperationId = "CreateWhatsappSession")]
    [SwaggerResponse(StatusCodes.Status201Created, "Session created", typeof(WhatsappSessionResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    public async Task<IActionResult> Create([FromBody] CreateWhatsappSessionResource resource,
        CancellationToken cancellationToken)
    {
        var command = new ReportBridgeConnectionCommand(false, null, resource.OwnerEmail,
            resource.BusinessName, resource.SellerId);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetAll), null,
                WhatsappSessionResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }

    [HttpPut("{id:int}")]
    [SwaggerOperation("Update the authenticated account's WhatsApp session",
        "Marks the session as connected or disconnected. Only ever updates the caller's own session.",
        OperationId = "UpdateWhatsappSession")]
    [SwaggerResponse(StatusCodes.Status200OK, "Session updated", typeof(WhatsappSessionResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Session not found")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateWhatsappSessionResource resource,
        CancellationToken cancellationToken)
    {
        var email = CurrentEmail();
        var sellerId = await iamFacade.FetchUserIdByEmail(email, cancellationToken);

        var existing = (await whatsappSessionQueryService.Handle(
            new GetAllWhatsappSessionsQuery(sellerId), cancellationToken)).FirstOrDefault();
        if (existing is null || existing.Id != id)
            return problemDetailsFactory.CreateProblemDetails(this, StatusCodes.Status404NotFound,
                ChatbotError.SessionNotFound, errorLocalizer[nameof(ChatbotError.SessionNotFound)]);

        var connected = string.Equals(resource.Status, "connected", StringComparison.OrdinalIgnoreCase);
        var businessName = string.IsNullOrWhiteSpace(resource.BusinessName) ? existing.BusinessName : resource.BusinessName;
        var command = new ReportBridgeConnectionCommand(connected, resource.Phone, email, businessName, sellerId);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            updated => Ok(WhatsappSessionResourceFromEntityAssembler.ToResourceFromEntity(updated)));
    }

    [HttpGet("qr")]
    [SwaggerOperation("Get or start the authenticated account's WhatsApp pairing QR",
        "Ensures a bridge session exists for the caller and returns its pairing QR / connection state. " +
        "The seller identity always comes from the JWT, never from the request, since the bridge itself " +
        "has no authentication of its own.",
        OperationId = "GetWhatsappBridgeQr")]
    [SwaggerResponse(StatusCodes.Status200OK, "QR data URL (or null) and connection state")]
    public async Task<IActionResult> GetBridgeQr(CancellationToken cancellationToken)
    {
        var email = CurrentEmail();
        var sellerId = await iamFacade.FetchUserIdByEmail(email, cancellationToken);

        var existing = (await whatsappSessionQueryService.Handle(
            new GetAllWhatsappSessionsQuery(sellerId), cancellationToken)).FirstOrDefault();
        var businessName = existing?.BusinessName ?? DefaultBusinessName;

        var (qr, connected) = await messagingService.GetOrStartSessionAsync(email, sellerId, businessName, cancellationToken);
        return Ok(new { qr, connected });
    }

    private string CurrentEmail() => (HttpContext.Items["User"] as User)?.Email ?? string.Empty;

    /// <summary>
    ///     Resolves the IAM user id of the authenticated caller, which doubles as the chatbot SellerId.
    ///     Never trusts a client-supplied seller id, so one account can never read another's session.
    /// </summary>
    private async Task<int> CurrentSellerIdAsync(CancellationToken cancellationToken)
    {
        return await iamFacade.FetchUserIdByEmail(CurrentEmail(), cancellationToken);
    }
}
