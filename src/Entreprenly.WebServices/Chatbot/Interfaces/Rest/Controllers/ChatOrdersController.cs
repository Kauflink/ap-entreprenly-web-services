using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Model;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;
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
[SwaggerTag("Chat order (pedidos) endpoints")]
public class ChatOrdersController(
    IChatOrderQueryService chatOrderQueryService,
    IChatOrderCommandService chatOrderCommandService,
    IStringLocalizer<ErrorMessages> errorLocalizer,
    ProblemDetailsFactory problemDetailsFactory)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Get all chat orders",
        "Returns every order captured from chatbot conversations, optionally filtered by seller.",
        OperationId = "GetAllChatOrders")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of chat orders", typeof(IEnumerable<ChatOrderResource>))]
    public async Task<IActionResult> GetAll([FromQuery] int? sellerId, CancellationToken cancellationToken)
    {
        var orders = await chatOrderQueryService.Handle(new GetAllChatOrdersQuery(sellerId), cancellationToken);
        return Ok(orders.Select(ChatOrderResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation("Get chat order by id", "Returns a single chat order by its identifier.",
        OperationId = "GetChatOrderById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The chat order", typeof(ChatOrderResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await chatOrderQueryService.Handle(new GetChatOrderByIdQuery(id), cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromNullable(
            this, order, errorLocalizer, problemDetailsFactory,
            ChatbotError.OrderNotFound,
            found => Ok(ChatOrderResourceFromEntityAssembler.ToResourceFromEntity(found)));
    }

    [HttpPost]
    [SwaggerOperation("Create a chat order", "Registers a new order captured from a chatbot conversation.",
        OperationId = "CreateChatOrder")]
    [SwaggerResponse(StatusCodes.Status201Created, "Order created", typeof(ChatOrderResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    public async Task<IActionResult> Create([FromBody] CreateChatOrderResource resource,
        CancellationToken cancellationToken)
    {
        var command = new CreateChatOrderCommand(resource.ConversationId, resource.SellerId,
            resource.OwnerEmail, resource.ClientPhone, resource.DeliveryAddress, resource.Items);
        var result = await chatOrderCommandService.Handle(command, cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            created => CreatedAtAction(nameof(GetById), new { id = created.Id },
                ChatOrderResourceFromEntityAssembler.ToResourceFromEntity(created)));
    }

    [HttpPost("{id:int}/confirm")]
    [SwaggerOperation("Confirm a chat order", "Confirms a chat order once its payment receipt has been validated.",
        OperationId = "ConfirmChatOrder")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order confirmed", typeof(ChatOrderResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
    public async Task<IActionResult> Confirm(int id, CancellationToken cancellationToken)
    {
        var result = await chatOrderCommandService.Handle(new ConfirmChatOrderCommand(id), cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            confirmed => Ok(ChatOrderResourceFromEntityAssembler.ToResourceFromEntity(confirmed)));
    }

    [HttpPost("{id:int}/reject")]
    [SwaggerOperation("Reject a chat order receipt", "Rejects the payment receipt attached to a chat order.",
        OperationId = "RejectChatOrder")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order rejected", typeof(ChatOrderResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectChatOrderResource resource,
        CancellationToken cancellationToken)
    {
        var result = await chatOrderCommandService.Handle(new RejectChatOrderCommand(id, resource.Reason), cancellationToken);
        return ChatbotActionResultAssembler.ToActionResultFromResult(
            this, result, errorLocalizer, problemDetailsFactory,
            rejected => Ok(ChatOrderResourceFromEntityAssembler.ToResourceFromEntity(rejected)));
    }
}
