using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.Internal.OutboundServices;
using Entreprenly.WebServices.Chatbot.Domain.Model;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Chatbot.Application.Internal.CommandServices;

public class ChatOrderCommandService(
    IChatOrderRepository chatOrderRepository,
    IConversationRepository conversationRepository,
    IChatMessageRepository chatMessageRepository,
    IWhatsappSessionRepository whatsappSessionRepository,
    IWhatsAppMessagingService messagingService,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IChatOrderCommandService
{
    public async Task<Result<ChatOrder>> Handle(CreateChatOrderCommand command, CancellationToken cancellationToken)
    {
        var order = new ChatOrder(command.ConversationId, command.SellerId, command.ClientPhone,
            command.DeliveryAddress, command.Items);

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
                $"✅ Pedido #{order.OrderNumber} confirmado.", MessageSender.System, MessageType.Text);
            await chatMessageRepository.AddAsync(sysMsg, cancellationToken);
        }

        await unitOfWork.CompleteAsync(cancellationToken);

        var session = await whatsappSessionRepository.FindBySellerIdAsync(order.SellerId, cancellationToken);
        if (session is not null)
        {
            var msg = $"✅ Tu pedido #{order.OrderNumber} fue confirmado. ¡Gracias por tu compra!";
            await messagingService.SendMessageAsync(session.OwnerEmail, order.ClientPhone, msg, cancellationToken);
        }

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

        var session = await whatsappSessionRepository.FindBySellerIdAsync(order.SellerId, cancellationToken);
        if (session is not null)
        {
            var msg = order.Status == OrderStatus.Blocked
                ? $"❌ Tu pedido #{order.OrderNumber} fue bloqueado por múltiples rechazos."
                : $"⚠️ Tu comprobante no pudo ser validado. Motivo: {command.Reason}. Por favor envía nuevamente.";
            await messagingService.SendMessageAsync(session.OwnerEmail, order.ClientPhone, msg, cancellationToken);
        }

        return Result<ChatOrder>.Success(order);
    }
}
