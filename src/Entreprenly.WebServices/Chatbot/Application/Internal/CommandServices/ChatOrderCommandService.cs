using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.Internal.OutboundServices;
using Entreprenly.WebServices.Chatbot.Domain.Model;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Chatbot.Resources;
using Entreprenly.WebServices.Sales.Interfaces.Acl;
using Entreprenly.WebServices.Shared.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Chatbot.Application.Internal.CommandServices;

/// <summary>
///     Handles chat order commands: creation, confirmation, and rejection of orders originated in chatbot conversations.
/// </summary>
public class ChatOrderCommandService(
    IChatOrderRepository chatOrderRepository,
    IConversationRepository conversationRepository,
    IChatMessageRepository chatMessageRepository,
    IWhatsAppMessagingService messagingService,
    ISalesContextFacade salesFacade,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer,
    IStringLocalizer<ChatbotMessages> botLocalizer)
    : IChatOrderCommandService
{
    public async Task<Result<ChatOrder>> Handle(CreateChatOrderCommand command, CancellationToken cancellationToken)
    {
        var order = new ChatOrder(command.ConversationId, command.SellerId, command.OwnerEmail,
            command.ClientPhone, command.Items);
        order.ConfirmDelivery(command.DeliveryAddress);

        await chatOrderRepository.AddAsync(order, cancellationToken);

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<ChatOrder>.Success(order);
        }
        catch (OperationCanceledException)
        {
            return Result<ChatOrder>.Failure(ChatbotError.OperationCancelled,
                localizer[nameof(ChatbotError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<ChatOrder>.Failure(ChatbotError.DatabaseError,
                localizer[nameof(ChatbotError.DatabaseError)]);
        }
    }

    public async Task<Result<ChatOrder>> Handle(ConfirmChatOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await chatOrderRepository.FindByIdAsync(command.ChatOrderId, cancellationToken);
        if (order is null)
            return Result<ChatOrder>.Failure(ChatbotError.OrderNotFound,
                localizer[nameof(ChatbotError.OrderNotFound)]);

        order.Confirm();
        chatOrderRepository.Update(order);

        var conversation = await conversationRepository.FindByIdAsync(order.ConversationId, cancellationToken);
        if (conversation is not null)
        {
            conversation.UpdateStatus(ConversationStatus.Completed);
            conversationRepository.Update(conversation);

            var sysMsg = new ChatMessage(conversation.Id,
                string.Format(botLocalizer["OrderConfirmedSystemMessage"].Value, order.OrderNumber),
                MessageSender.System, MessageType.Text);
            await chatMessageRepository.AddAsync(sysMsg, cancellationToken);
        }

        await unitOfWork.CompleteAsync(cancellationToken);

        var lines = order.Items
            .Select(i => new ChatSaleLine(i.ProductName, (int)i.Quantity, (double)i.UnitPrice))
            .ToList();
        await salesFacade.RegisterChatSale(order.OwnerEmail, order.SellerId, lines, (double)order.Total,
            cancellationToken);

        var confirmMsg = string.Format(botLocalizer["OrderConfirmedClientMessage"].Value, order.OrderNumber);
        await messagingService.SendMessageAsync(order.OwnerEmail, order.ClientPhone, confirmMsg, cancellationToken);

        return Result<ChatOrder>.Success(order);
    }

    public async Task<Result<ChatOrder>> Handle(RejectChatOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await chatOrderRepository.FindByIdAsync(command.ChatOrderId, cancellationToken);
        if (order is null)
            return Result<ChatOrder>.Failure(ChatbotError.OrderNotFound,
                localizer[nameof(ChatbotError.OrderNotFound)]);

        order.Reject();
        chatOrderRepository.Update(order);

        var conversation = await conversationRepository.FindByIdAsync(order.ConversationId, cancellationToken);
        if (conversation is not null && order.Status == OrderStatus.Blocked)
        {
            conversation.UpdateStatus(ConversationStatus.Closed);
            conversationRepository.Update(conversation);
        }

        await unitOfWork.CompleteAsync(cancellationToken);

        var rejectMsg = order.Status == OrderStatus.Blocked
            ? string.Format(botLocalizer["OrderBlockedMessage"].Value, order.OrderNumber)
            : string.Format(botLocalizer["ReceiptRejectedMessage"].Value, command.Reason);
        await messagingService.SendMessageAsync(order.OwnerEmail, order.ClientPhone, rejectMsg, cancellationToken);

        return Result<ChatOrder>.Success(order);
    }
}
