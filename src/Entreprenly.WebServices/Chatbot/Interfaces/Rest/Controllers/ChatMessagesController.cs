using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Interfaces.Rest.ProblemDetails;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Chat message endpoints")]
public class ChatMessagesController(
    IChatMessageQueryService chatMessageQueryService,
    IChatbotConversationService chatbotConversationService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Get all chat messages", OperationId = "GetAllChatMessages")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of messages", typeof(IEnumerable<ChatMessageResource>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var messages = await chatMessageQueryService.Handle(new GetAllChatMessagesQuery(), cancellationToken);
        return Ok(messages.Select(ChatMessageResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("by-conversation/{conversationId:int}")]
    [SwaggerOperation("Get messages by conversation", OperationId = "GetChatMessagesByConversation")]
    [SwaggerResponse(StatusCodes.Status200OK, "Messages for the conversation", typeof(IEnumerable<ChatMessageResource>))]
    public async Task<IActionResult> GetByConversation(int conversationId, CancellationToken cancellationToken)
    {
        var messages = await chatMessageQueryService.Handle(
            new GetChatMessagesByConversationIdQuery(conversationId), cancellationToken);
        return Ok(messages.Select(ChatMessageResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpPost]
    [SwaggerOperation("Create a chat message", OperationId = "CreateChatMessage")]
    [SwaggerResponse(StatusCodes.Status201Created, "Message created", typeof(ChatMessageResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Conversation not found")]
    public async Task<IActionResult> Create([FromBody] CreateChatMessageResource resource,
        CancellationToken cancellationToken)
    {
        var command = new CreateManualMessageCommand(
            resource.ConversationId, resource.Content, resource.Sender, resource.Type);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetByConversation),
                new { conversationId = created.ConversationId },
                ChatMessageResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }
}
