using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.Internal.OutboundServices;
using Entreprenly.WebServices.Chatbot.Domain.Model;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Chatbot.Domain.Services;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Chatbot.Application.Internal.CommandServices;

public class ChatbotConversationService(
    IConversationRepository conversationRepository,
    IChatMessageRepository chatMessageRepository,
    IChatOrderRepository chatOrderRepository,
    IWhatsappSessionRepository whatsappSessionRepository,
    IChatbotResponder chatbotResponder,
    ProductReplyComposer productComposer,
    IWhatsAppMessagingService messagingService,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IChatbotConversationService
{
    public async Task<Result<string?>> Handle(HandleInboundMessageCommand command, CancellationToken cancellationToken)
    {
        var session = await whatsappSessionRepository.FindByOwnerEmailAsync(command.OwnerEmail, cancellationToken);
        if (session is null)
            return Result<string?>.Failure(ChatbotError.SessionNotFound,
                localizer[nameof(ChatbotError.SessionNotFound)]);

        var conversation = await conversationRepository.FindByClientPhoneAndSellerIdAsync(
            command.FromPhone, session.SellerId, cancellationToken);

        if (conversation is null)
        {
            conversation = new Conversation(session.SellerId, command.FromPhone, command.ClientName);
            await conversationRepository.AddAsync(conversation, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);
        }

        var clientMessage = new ChatMessage(conversation.Id, command.Content, MessageSender.Client, MessageType.Text);
        await chatMessageRepository.AddAsync(clientMessage, cancellationToken);

        var reply = await ComposeReplyAsync(command.Content, conversation, session.SellerId, cancellationToken);

        if (reply is not null)
        {
            var botMessage = new ChatMessage(conversation.Id, reply, MessageSender.Bot, MessageType.Text);
            await chatMessageRepository.AddAsync(botMessage, cancellationToken);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            return Result<string?>.Failure(ChatbotError.OperationCancelled,
                localizer[nameof(ChatbotError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<string?>.Failure(ChatbotError.DatabaseError,
                localizer[nameof(ChatbotError.DatabaseError)]);
        }

        if (reply is not null)
            await messagingService.SendMessageAsync(command.OwnerEmail, command.FromPhone, reply, cancellationToken);

        return Result<string?>.Success(reply);
    }

    public async Task<Result<string?>> Handle(HandleInboundReceiptCommand command, CancellationToken cancellationToken)
    {
        var session = await whatsappSessionRepository.FindByOwnerEmailAsync(command.OwnerEmail, cancellationToken);
        if (session is null)
            return Result<string?>.Failure(ChatbotError.SessionNotFound,
                localizer[nameof(ChatbotError.SessionNotFound)]);

        var conversation = await conversationRepository.FindByClientPhoneAndSellerIdAsync(
            command.FromPhone, session.SellerId, cancellationToken);

        if (conversation is null)
            return Result<string?>.Failure(ChatbotError.ConversationNotFound,
                localizer[nameof(ChatbotError.ConversationNotFound)]);

        var order = await chatOrderRepository.FindWaitingPaymentByConversationIdAsync(
            conversation.Id, cancellationToken);

        if (order is not null)
        {
            order.AttachReceipt(command.Image);
            chatOrderRepository.Update(order);
        }

        var sysMessage = new ChatMessage(conversation.Id,
            "[Comprobante recibido]", MessageSender.System, MessageType.Image);
        await chatMessageRepository.AddAsync(sysMessage, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        const string confirmReply = "✅ ¡Recibí tu comprobante! Lo estamos validando y te confirmamos en breve. 🙌";
        await messagingService.SendMessageAsync(command.OwnerEmail, command.FromPhone, confirmReply, cancellationToken);

        return Result<string?>.Success(confirmReply);
    }

    public async Task<Result<Conversation>> Handle(CreateConversationCommand command, CancellationToken cancellationToken)
    {
        var conversation = new Conversation(command.SellerId, command.ClientPhone, command.ClientName);
        await conversationRepository.AddAsync(conversation, cancellationToken);

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<Conversation>.Success(conversation);
        }
        catch (OperationCanceledException)
        {
            return Result<Conversation>.Failure(ChatbotError.OperationCancelled,
                localizer[nameof(ChatbotError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<Conversation>.Failure(ChatbotError.DatabaseError,
                localizer[nameof(ChatbotError.DatabaseError)]);
        }
    }

    public async Task<Result<Conversation>> Handle(UpdateConversationCommand command, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.FindByIdAsync(command.ConversationId, cancellationToken);
        if (conversation is null)
            return Result<Conversation>.Failure(ChatbotError.ConversationNotFound,
                localizer[nameof(ChatbotError.ConversationNotFound)]);

        conversation.UpdateStatus(command.Status);
        conversationRepository.Update(conversation);

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Conversation>.Success(conversation);
    }

    public async Task<Result<ChatMessage>> Handle(CreateManualMessageCommand command,
        CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.FindByIdAsync(command.ConversationId, cancellationToken);
        if (conversation is null)
            return Result<ChatMessage>.Failure(ChatbotError.ConversationNotFound,
                localizer[nameof(ChatbotError.ConversationNotFound)]);

        if (!Enum.TryParse<MessageSender>(command.Sender, true, out var sender))
            return Result<ChatMessage>.Failure(ChatbotError.InternalServerError,
                localizer[nameof(ChatbotError.InternalServerError)]);

        if (!Enum.TryParse<MessageType>(command.Type, true, out var type))
            return Result<ChatMessage>.Failure(ChatbotError.InternalServerError,
                localizer[nameof(ChatbotError.InternalServerError)]);

        var message = new ChatMessage(command.ConversationId, command.Content, sender, type);
        await chatMessageRepository.AddAsync(message, cancellationToken);

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<ChatMessage>.Success(message);
        }
        catch (OperationCanceledException)
        {
            return Result<ChatMessage>.Failure(ChatbotError.OperationCancelled,
                localizer[nameof(ChatbotError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<ChatMessage>.Failure(ChatbotError.DatabaseError,
                localizer[nameof(ChatbotError.DatabaseError)]);
        }
    }

    public async Task<Result<WhatsappSession>> Handle(ReportBridgeConnectionCommand command,
        CancellationToken cancellationToken)
    {
        var session = await whatsappSessionRepository.FindByOwnerEmailAsync(command.OwnerEmail, cancellationToken);

        if (session is null)
        {
            session = new WhatsappSession(command.SellerId, command.OwnerEmail, command.BusinessName);
            if (command.Connected && command.Phone is not null)
                session.ReportConnected(command.Phone);
            await whatsappSessionRepository.AddAsync(session, cancellationToken);
        }
        else
        {
            if (command.Connected && command.Phone is not null)
                session.ReportConnected(command.Phone);
            else
                session.ReportDisconnected();

            whatsappSessionRepository.Update(session);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
            return Result<WhatsappSession>.Success(session);
        }
        catch (OperationCanceledException)
        {
            return Result<WhatsappSession>.Failure(ChatbotError.OperationCancelled,
                localizer[nameof(ChatbotError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<WhatsappSession>.Failure(ChatbotError.DatabaseError,
                localizer[nameof(ChatbotError.DatabaseError)]);
        }
    }

    private async Task<string?> ComposeReplyAsync(
        string text, Conversation conversation, int sellerId, CancellationToken ct)
    {
        // 1. Pending order waiting for delivery address
        var pendingOrder = await chatOrderRepository.FindPendingByConversationIdAsync(conversation.Id, ct);
        if (pendingOrder is not null && LooksLikeAddress(text))
        {
            pendingOrder.ConfirmDelivery(text.Trim());
            chatOrderRepository.Update(pendingOrder);
            return $"¡Listo! 📍 Registré tu pedido *{pendingOrder.OrderNumber}* con entrega en '{text.Trim()}'.\n\n" +
                   "Ahora envíame la captura de tu pago (Yape, Plin o transferencia). 📸";
        }

        // 2. Product detection from catalog
        var (items, productReply) = await productComposer.TryComposeAsync(text, conversation.Id, sellerId, ct);
        if (items is not null)
        {
            var order = new ChatOrder(conversation.Id, sellerId, conversation.ClientPhone, items);
            await chatOrderRepository.AddAsync(order, ct);
            return productReply;
        }
        if (productReply is not null) return productReply;

        // 3. Keyword rule-based fallback
        return await chatbotResponder.GenerateReplyAsync(text, sellerId, ct);
    }

    private static bool LooksLikeAddress(string text)
    {
        var lower = text.ToLowerInvariant().Trim();
        if (lower.Length <= 4) return false;
        var greetings = new[] { "hola", "hi", "ok", "si", "sí", "no", "gracias", "bueno", "bien", "claro" };
        return !greetings.Contains(lower);
    }
}
