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
[SwaggerTag("Chatbot conversation endpoints")]
public class ConversationsController(
    IConversationQueryService conversationQueryService,
    IChatbotConversationService chatbotConversationService)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Get all conversations", OperationId = "GetAllConversations")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of conversations", typeof(IEnumerable<ConversationResource>))]
    public async Task<IActionResult> GetAll([FromQuery] int? sellerId, CancellationToken cancellationToken)
    {
        var conversations = await conversationQueryService.Handle(new GetAllConversationsQuery(sellerId), cancellationToken);
        return Ok(conversations.Select(ConversationResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation("Get conversation by id", OperationId = "GetConversationById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The conversation", typeof(ConversationResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Conversation not found")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var conversation = await conversationQueryService.Handle(new GetConversationByIdQuery(id), cancellationToken);
        if (conversation is null) return NotFound();
        return Ok(ConversationResourceFromEntityAssembler.ToResourceFromEntity(conversation));
    }

    [HttpPost]
    [SwaggerOperation("Create a conversation", OperationId = "CreateConversation")]
    [SwaggerResponse(StatusCodes.Status201Created, "Conversation created", typeof(ConversationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    public async Task<IActionResult> Create([FromBody] CreateConversationResource resource,
        CancellationToken cancellationToken)
    {
        var command = new CreateConversationCommand(resource.SellerId, resource.ClientPhone, resource.ClientName);
        var result  = await chatbotConversationService.Handle(command, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
            ConversationResourceFromEntityAssembler.ToResourceFromEntity(result.Value));
    }
}
