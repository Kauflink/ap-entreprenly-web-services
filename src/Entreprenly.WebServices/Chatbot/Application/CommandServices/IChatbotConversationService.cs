using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;

namespace Entreprenly.WebServices.Chatbot.Application.CommandServices;

public interface IChatbotConversationService
{
    Task<Result<string?>> Handle(HandleInboundMessageCommand command, CancellationToken cancellationToken);
    Task<Result<string?>> Handle(HandleInboundReceiptCommand command, CancellationToken cancellationToken);
    Task<Result<Conversation>> Handle(CreateConversationCommand command, CancellationToken cancellationToken);
    Task<Result<Conversation>> Handle(UpdateConversationCommand command, CancellationToken cancellationToken);
    Task<Result<WhatsappSession>> Handle(ReportBridgeConnectionCommand command, CancellationToken cancellationToken);
    Task<Result<ChatMessage>> Handle(CreateManualMessageCommand command, CancellationToken cancellationToken);
}
