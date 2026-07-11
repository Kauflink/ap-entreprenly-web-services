using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.QueryServices;
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
    IIamContextFacade iamFacade,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
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

    /// <summary>
    ///     Resolves the IAM user id of the authenticated caller, which doubles as the chatbot SellerId.
    ///     Never trusts a client-supplied seller id, so one account can never read another's session.
    /// </summary>
    private async Task<int> CurrentSellerIdAsync(CancellationToken cancellationToken)
    {
        var email = (HttpContext.Items["User"] as User)?.Email ?? string.Empty;
        return await iamFacade.FetchUserIdByEmail(email, cancellationToken);
    }
}
