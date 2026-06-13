using System.Net.Mime;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Domain.Model;
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
[SwaggerTag("Chat order (pedidos) endpoints")]
public class ChatOrdersController(
    IChatOrderQueryService chatOrderQueryService,
    IChatOrderCommandService chatOrderCommandService)
    : ControllerBase
{
    [HttpGet]
    [SwaggerOperation("Get all chat orders", OperationId = "GetAllChatOrders")]
    [SwaggerResponse(StatusCodes.Status200OK, "List of chat orders", typeof(IEnumerable<ChatOrderResource>))]
    public async Task<IActionResult> GetAll([FromQuery] int? sellerId, CancellationToken cancellationToken)
    {
        var orders = await chatOrderQueryService.Handle(new GetAllChatOrdersQuery(sellerId), cancellationToken);
        return Ok(orders.Select(ChatOrderResourceFromEntityAssembler.ToResourceFromEntity));
    }

    [HttpGet("{id:int}")]
    [SwaggerOperation("Get chat order by id", OperationId = "GetChatOrderById")]
    [SwaggerResponse(StatusCodes.Status200OK, "The chat order", typeof(ChatOrderResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var order = await chatOrderQueryService.Handle(new GetChatOrderByIdQuery(id), cancellationToken);
        if (order is null) return NotFound();
        return Ok(ChatOrderResourceFromEntityAssembler.ToResourceFromEntity(order));
    }

    [HttpPost]
    [SwaggerOperation("Create a chat order", OperationId = "CreateChatOrder")]
    [SwaggerResponse(StatusCodes.Status201Created, "Order created", typeof(ChatOrderResource))]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "Invalid request")]
    public async Task<IActionResult> Create([FromBody] CreateChatOrderResource resource,
        CancellationToken cancellationToken)
    {
        var command = new CreateChatOrderCommand(resource.ConversationId, resource.SellerId,
            resource.ClientPhone, resource.DeliveryAddress, resource.Items);
        var result = await chatOrderCommandService.Handle(command, cancellationToken);
        if (!result.IsSuccess) return BadRequest(result.Error);
        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id },
            ChatOrderResourceFromEntityAssembler.ToResourceFromEntity(result.Value));
    }

    [HttpPost("{id:int}/confirm")]
    [SwaggerOperation("Confirm a chat order", OperationId = "ConfirmChatOrder")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order confirmed", typeof(ChatOrderResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
    public async Task<IActionResult> Confirm(int id, CancellationToken cancellationToken)
    {
        var result = await chatOrderCommandService.Handle(new ConfirmChatOrderCommand(id), cancellationToken);
        if (!result.IsSuccess)
            return result.Error is ChatbotError.OrderNotFound ? NotFound() : BadRequest(result.Message);
        return Ok(ChatOrderResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }

    [HttpPost("{id:int}/reject")]
    [SwaggerOperation("Reject a chat order receipt", OperationId = "RejectChatOrder")]
    [SwaggerResponse(StatusCodes.Status200OK, "Order rejected", typeof(ChatOrderResource))]
    [SwaggerResponse(StatusCodes.Status404NotFound, "Order not found")]
    public async Task<IActionResult> Reject(int id, [FromBody] RejectChatOrderResource resource,
        CancellationToken cancellationToken)
    {
        var result = await chatOrderCommandService.Handle(new RejectChatOrderCommand(id, resource.Reason), cancellationToken);
        if (!result.IsSuccess)
            return result.Error is ChatbotError.OrderNotFound ? NotFound() : BadRequest(result.Message);
        return Ok(ChatOrderResourceFromEntityAssembler.ToResourceFromEntity(result.Value!));
    }
}
