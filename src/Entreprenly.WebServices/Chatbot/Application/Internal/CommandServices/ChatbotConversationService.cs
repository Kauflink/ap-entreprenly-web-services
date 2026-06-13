using Entreprenly.WebServices.Chatbot.Application.CommandServices;
using Entreprenly.WebServices.Chatbot.Application.Internal.OutboundServices;
using Entreprenly.WebServices.Chatbot.Domain.Model;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Chatbot.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;
using Entreprenly.WebServices.Chatbot.Domain.Services;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Entreprenly.WebServices.Chatbot.Application.Internal.CommandServices;

public class ChatbotConversationService(
    IConversationRepository conversationRepository,
    IChatMessageRepository chatMessageRepository,
    IWhatsappSessionRepository whatsappSessionRepository,
    IChatbotResponder chatbotResponder,
    IWhatsAppMessagingService messagingService,
    IUnitOfWork unitOfWork)
    : IChatbotConversationService
{
    public async Task<Result<string?>> Handle(HandleInboundMessageCommand command, CancellationToken cancellationToken)
    {
        var session = await whatsappSessionRepository.FindByOwnerEmailAsync(command.OwnerEmail, cancellationToken);
        if (session is null)
            return Result<string?>.Failure(ChatbotError.SessionNotFound, "No active WhatsApp session for this seller.");

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

        var reply = await chatbotResponder.GenerateReplyAsync(command.Content, session.SellerId, cancellationToken);

        if (reply is not null)
        {
            var botMessage = new ChatMessage(conversation.Id, reply, MessageSender.Bot, MessageType.Text);
            await chatMessageRepository.AddAsync(botMessage, cancellationToken);
        }

        try
        {
            await unitOfWork.CompleteAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            return Result<string?>.Failure(ChatbotError.DatabaseError, "Could not persist message.");
        }

        if (reply is not null)
            await messagingService.SendMessageAsync(command.OwnerEmail, command.FromPhone, reply, cancellationToken);

        return Result<string?>.Success(reply);
    }

    public async Task<Result<string?>> Handle(HandleInboundReceiptCommand command, CancellationToken cancellationToken)
    {
        var session = await whatsappSessionRepository.FindByOwnerEmailAsync(command.OwnerEmail, cancellationToken);
        if (session is null)
            return Result<string?>.Failure(ChatbotError.SessionNotFound, "No active session.");

        var conversation = await conversationRepository.FindByClientPhoneAndSellerIdAsync(
            command.FromPhone, session.SellerId, cancellationToken);

        if (conversation is null)
            return Result<string?>.Failure(ChatbotError.ConversationNotFound, "No active conversation for this client.");

        var sysMessage = new ChatMessage(conversation.Id,
            $"[Comprobante recibido]", MessageSender.System, MessageType.Image);
        await chatMessageRepository.AddAsync(sysMessage, cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);

        const string confirmReply = "✅ Comprobante recibido. Estamos validando tu pago.";
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
        catch (DbUpdateException)
        {
            return Result<Conversation>.Failure(ChatbotError.DatabaseError, "Could not create conversation.");
        }
    }

    public async Task<Result<Conversation>> Handle(UpdateConversationCommand command, CancellationToken cancellationToken)
    {
        var conversation = await conversationRepository.FindByIdAsync(command.ConversationId, cancellationToken);
        if (conversation is null)
            return Result<Conversation>.Failure(ChatbotError.ConversationNotFound, "Conversation not found.");

        conversation.UpdateStatus(command.Status);
        conversationRepository.Update(conversation);

        await unitOfWork.CompleteAsync(cancellationToken);
        return Result<Conversation>.Success(conversation);
    }

    public async Task<Result<WhatsappSession>> Handle(ReportBridgeConnectionCommand command,
        CancellationToken cancellationToken)
    {
        var session = await whatsappSessionRepository.FindByOwnerEmailAsync(command.OwnerEmail, cancellationToken);

        if (session is null)
        {
            session = new WhatsappSession(command.SellerId, command.OwnerEmail, command.BusinessName);
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
        catch (DbUpdateException)
        {
            return Result<WhatsappSession>.Failure(ChatbotError.DatabaseError, "Could not update session.");
        }
    }
}
