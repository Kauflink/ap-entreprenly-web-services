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
[SwaggerTag("Chatbot conversation endpoints")]
public class ConversationsController(
    IConversationQueryService conversationQueryService,
    IChatbotConversationService chatbotConversationService,
    IIamContextFacade iamFacade,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Get all conversations",
        "Returns every chatbot conversation belonging to the authenticated account, with its latest message.",
        OperationId = "GetAllConversations")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of conversations", typeof(IEnumerable<ConversationResource>))]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var sellerId = await CurrentSellerIdAsync(cancellationToken);
        var pairs = await conversationQueryService.Handle(new GetAllConversationsQuery(sellerId), cancellationToken);
        var resources = pairs.Select(p =>
            ConversationResourceFromEntityAssembler.ToResourceFromEntity(
                p.Item1, p.Item2?.Content, p.Item2?.SentAt));
        return Ok(resources);
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation("Get conversation by id", "Returns a single chatbot conversation by its identifier.",
        OperationId = "GetConversationById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The conversation", typeof(ConversationResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Conversation not found")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var conversation = await conversationQueryService.Handle(new GetConversationByIdQuery(id), cancellationToken);
        var sellerId = await CurrentSellerIdAsync(cancellationToken);
        if (conversation is not null && conversation.SellerId != sellerId) conversation = null;
        return ChatbotActionResultAssembler.ToActionResultFromNullable(
            this, conversation, errorLocalizer, problemDetailsFactory,
            ChatbotError.ConversationNotFound,
            found => Ok(ConversationResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    /// <summary>
    ///     Resolves the IAM user id of the authenticated caller, which doubles as the chatbot SellerId.
    ///     Never trusts a client-supplied seller id, so one account can never read another's conversations.
    /// </summary>
    private async Task<int> CurrentSellerIdAsync(CancellationToken cancellationToken)
    {
        var email = (HttpContext.Items["User"] as User)?.Email ?? string.Empty;
        return await iamFacade.FetchUserIdByEmail(email, cancellationToken);
    }

    [HttpPost]
    [SwaggerOperation("Create a conversation", "Starts a new chatbot conversation with a customer.",
        OperationId = "CreateConversation")]
    [SwaggerResponse(StatusCodes.Status201Created, "Conversation created", typeof(ConversationResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    public async Task<IActionResult> Create([FromBody] CreateConversationResource resource,
        CancellationToken cancellationToken)
    {
        var command = new CreateConversationCommand(resource.SellerId, resource.ClientPhone, resource.ClientName);
        var result = await chatbotConversationService.Handle(command, cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetById), new { id = created.Id },
                ConversationResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }
}
