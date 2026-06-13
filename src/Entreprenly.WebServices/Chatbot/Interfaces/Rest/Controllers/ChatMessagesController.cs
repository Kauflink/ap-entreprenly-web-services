using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.QueryServices;
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
[SwaggerTag("Chat message endpoints")]
public class ChatMessagesController(IChatMessageQueryService chatMessageQueryService) : ControllerBase
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
}
