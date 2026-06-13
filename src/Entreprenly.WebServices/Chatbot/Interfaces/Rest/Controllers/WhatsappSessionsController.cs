using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("WhatsApp session endpoints")]
public class WhatsappSessionsController(
    IWhatsappSessionQueryService whatsappSessionQueryService,
    IChatbotConversationService chatbotConversationService)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Get all WhatsApp sessions", OperationId = "GetAllWhatsappSessions")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of sessions", typeof(IEnumerable<WhatsappSessionResource>))]
    public async Task<IActionResult> GetAll([FromQuery] int? sellerId, CancellationToken cancellationToken)
    {
        var sessions = await whatsappSessionQueryService.Handle(new GetAllWhatsappSessionsQuery(sellerId), cancellationToken);
        return Ok(sessions.Select(WhatsappSessionResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [SwaggerOperation("Create a WhatsApp session", OperationId = "CreateWhatsappSession")]
    [SwaggerResponse(StatusCodes.Status201Created, "Session created", typeof(WhatsappSessionResource))]
    public async Task<IActionResult> Create([FromBody] CreateWhatsappSessionResource resource,
        CancellationToken cancellationToken)
    {
        var command = new ReportBridgeConnectionCommand(false, null, resource.OwnerEmail,
            resource.BusinessName, resource.SellerId);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return Created(string.Empty, WhatsappSessionResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
}
