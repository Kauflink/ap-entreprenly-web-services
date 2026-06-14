using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[SwaggerTag("Chat message endpoints")]
public class ChatMessagesController(
    IChatMessageQueryService chatMessageQueryService,
    IChatMessageRepository chatMessageRepository,
    IUnitOfWork unitOfWork)
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
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid sender or type")]
    public async Task<IActionResult> Create([FromBody] CreateChatMessageResource resource,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<MessageSender>(resource.Sender, true, out var sender))
            return BadRequest(new { error = $"Invalid sender: {resource.Sender}" });

        if (!Enum.TryParse<MessageType>(resource.Type, true, out var type))
            return BadRequest(new { error = $"Invalid type: {resource.Type}" });

        var message = new ChatMessage(resource.ConversationId, resource.Content, sender, type);
        await chatMessageRepository.AddAsync(message, cancellationToken);
        await unitOfWork.CompleteAsync(cancellationToken);

        return CreatedAtAction(nameof(GetByConversation), new { conversationId = message.ConversationId },
            ChatMessageResourceFromEntityAssembler.ToResourceFromEntity(message));
    }
}
