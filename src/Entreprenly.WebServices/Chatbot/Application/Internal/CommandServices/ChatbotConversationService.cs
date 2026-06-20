using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.Internal.OutboundServices;
using Entreprenly.WebServices.Chatbot.Domain.Model;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Chatbot.Domain.Services;
using Entreprenly.WebServices.Iam.Application.QueryServices;
using Entreprenly.WebServices.Iam.Domain.Model.Queries;
using Entreprenly.WebServices.Profiles.Application.QueryServices;
using Entreprenly.WebServices.Profiles.Domain.Model.Queries;
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
    IUserQueryService userQueryService,
    IProfileQueryService profileQueryService,
    IUnitOfWork unitOfWork,
    IStringLocalizer<ErrorMessages> localizer)
    : IChatbotConversationService
{
    private static readonly ConcurrentDictionary<int, CatalogProduct> _lastProductByConversation = new();

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

        // The inbound message is always recorded, but the bot only auto-replies when the owner keeps the
        // chatbot notification enabled. When disabled the owner answers manually from the panel.
        var reply = await BotEnabledAsync(command.OwnerEmail, cancellationToken)
            ? await ComposeReplyAsync(command.Content, conversation, session.OwnerEmail, cancellationToken)
            : null;

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

        bool isFirstReceipt = false;
        if (order is not null)
        {
            isFirstReceipt = !order.HasReceipt;
            order.AttachReceipt(command.Image);
            chatOrderRepository.Update(order);
        }

        if (isFirstReceipt)
        {
            var sysMessage = new ChatMessage(conversation.Id,
                "[Comprobante recibido]", MessageSender.System, MessageType.Image);
            await chatMessageRepository.AddAsync(sysMessage, cancellationToken);
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

        // Receipt is always captured; the auto-acknowledgement is only sent when the bot is enabled.
        if (!await BotEnabledAsync(command.OwnerEmail, cancellationToken))
            return Result<string?>.Success(null);

        const string confirmReply = "Recibi tu comprobante. Lo estamos validando y te confirmamos en breve.";
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

    /// <summary>
    ///     Resolves whether the owner enabled the chatbot auto-reply. Fails open (keeps replying) when the
    ///     owner or profile cannot be resolved, so a lookup miss never silences the bot.
    /// </summary>
    private async Task<bool> BotEnabledAsync(string ownerEmail, CancellationToken ct)
    {
        var user = await userQueryService.Handle(new GetUserByEmailQuery(ownerEmail), ct);
        if (user is null) return true;

        var profile = await profileQueryService.Handle(new GetProfileByUserIdQuery(user.Id), ct);
        return profile?.NotificationSettings.ChatbotMessages ?? true;
    }

    private async Task<string?> ComposeReplyAsync(
        string text, Conversation conversation, string ownerEmail, CancellationToken ct)
    {
        var catalog = await productComposer.FetchCatalogAsync(ownerEmail, ct);

        // 1. Direct order detection (product + quantity + intent keyword)
        var directOrder = productComposer.DetectOrder(text, catalog);
        if (directOrder is not null)
        {
            _lastProductByConversation.TryRemove(conversation.Id, out _);
            return await RegisterDraftOrderAsync(conversation, ownerEmail, directOrder, ct);
        }

        // 2. Pending order waiting for delivery address
        var pendingOrder = await chatOrderRepository.FindPendingByConversationIdAsync(conversation.Id, ct);
        if (pendingOrder is not null && LooksLikeAddress(text))
        {
            _lastProductByConversation.TryRemove(conversation.Id, out _);
            pendingOrder.ConfirmDelivery(text.Trim());
            chatOrderRepository.Update(pendingOrder);
            return $"¡Listo! Registré tu pedido {pendingOrder.OrderNumber} con entrega en \"{text.Trim()}\". " +
                   "Ahora envíame la captura de tu pago (Yape/Plin) para validarlo.";
        }

        // 3. Contextual order (quantity for the last mentioned product)
        if (_lastProductByConversation.TryGetValue(conversation.Id, out var contextProduct))
        {
            var contextualOrder = productComposer.DetectOrder(text, catalog, contextProduct);
            if (contextualOrder is not null)
            {
                _lastProductByConversation.TryRemove(conversation.Id, out _);
                return await RegisterDraftOrderAsync(conversation, ownerEmail, contextualOrder, ct);
            }
        }

        // 4. Informational product reply (price/stock/catalogue)
        var productReply = productComposer.Compose(text, catalog);
        if (productReply is not null)
        {
            var matched = productComposer.MatchProduct(text, catalog);
            if (matched is not null) _lastProductByConversation[conversation.Id] = matched;
            return productReply;
        }

        // 5. Keyword rule-based fallback
        return await chatbotResponder.GenerateReplyAsync(text, conversation.ClientName, ct);
    }

    private async Task<string> RegisterDraftOrderAsync(Conversation conversation, string ownerEmail, OrderItem item, CancellationToken ct)
    {
        var order = new ChatOrder(conversation.Id, conversation.SellerId, ownerEmail, conversation.ClientPhone, [item]);
        await chatOrderRepository.AddAsync(order, ct);
        double total = Math.Round((double)item.Subtotal * 100.0) / 100.0;
        var unitLabel = item.Quantity == Math.Floor(item.Quantity) ? "unidades" : "kg";
        return $"Anotado tu pedido {order.OrderNumber}: {item.Quantity:0.#} {unitLabel} de {item.ProductName} = S/{total:0.00}. ¿A qué dirección te lo enviamos?";
    }

    private static bool LooksLikeAddress(string text)
    {
        var lower = text.Trim().ToLowerInvariant();
        if (lower.Length < 3) return false;
        return !Regex.IsMatch(lower, @"^(hola|buenas|buenos dias|buenas tardes|buenas noches|gracias|ok|si|no)\.?$");
    }
}
