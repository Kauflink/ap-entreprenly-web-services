using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
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
[SwaggerTag("Chat message endpoints")]
public class ChatMessagesController(
    IChatMessageQueryService chatMessageQueryService,
    IConversationQueryService conversationQueryService,
    IChatbotConversationService chatbotConversationService,
    IIamContextFacade iamFacade,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Get all chat messages",
        "Returns every chat message recorded across the authenticated account's conversations.",
        OperationId = "GetAllChatMessages")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of messages", typeof(IEnumerable<ChatMessageResource>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var sellerId = await CurrentSellerIdAsync(cancellationToken);
        var messages = await chatMessageQueryService.Handle(new GetAllChatMessagesQuery(sellerId), cancellationToken);
        return Ok(messages.Select(ChatMessageResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("by-conversation/{conversationId:int}")]
    [SwaggerOperation("Get messages by conversation",
        "Returns the chat messages that belong to the specified conversation.",
        OperationId = "GetChatMessagesByConversation")]
    [SwaggerResponse(StatusCodes.Status200OK, "Messages for the conversation", typeof(IEnumerable<ChatMessageResource>))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Conversation not found")]
    public async Task<IActionResult> GetByConversation(int conversationId, CancellationToken cancellationToken)
    {
        var conversation = await conversationQueryService.Handle(
            new GetConversationByIdQuery(conversationId), cancellationToken);
        var sellerId = await CurrentSellerIdAsync(cancellationToken);
        if (conversation is null || conversation.SellerId != sellerId)
            return problemDetailsFactory.CreateProblemDetails(this, StatusCodes.Status404NotFound,
                ChatbotError.ConversationNotFound, errorLocalizer[nameof(ChatbotError.ConversationNotFound)]);

        var messages = await chatMessageQueryService.Handle(
            new GetChatMessagesByConversationIdQuery(conversationId), cancellationToken);
        return Ok(messages.Select(ChatMessageResourceFromEntityAssembler.ToResourceFromEntity));
    }

    /// <summary>
    ///     Resolves the IAM user id of the authenticated caller, which doubles as the chatbot SellerId.
    ///     Never trusts a client-supplied seller id, so one account can never read another's messages.
    /// </summary>
    private async Task<int> CurrentSellerIdAsync(CancellationToken cancellationToken)
    {
        var email = (HttpContext.Items["User"] as User)?.Email ?? string.Empty;
        return await iamFacade.FetchUserIdByEmail(email, cancellationToken);
    }

    [HttpPost]
    [SwaggerOperation("Create a chat message", "Records a new chat message in a conversation.",
        OperationId = "CreateChatMessage")]
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
