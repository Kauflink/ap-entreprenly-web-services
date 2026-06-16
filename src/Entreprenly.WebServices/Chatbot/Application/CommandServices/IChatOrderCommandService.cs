using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;

namespace Entreprenly.WebServices.Chatbot.Application.CommandServices;

public interface IChatOrderCommandService
{
    Task<Result<ChatOrder>> Handle(CreateChatOrderCommand command, CancellationToken cancellationToken);
    Task<Result<ChatOrder>> Handle(ConfirmChatOrderCommand command, CancellationToken cancellationToken);
    Task<Result<ChatOrder>> Handle(RejectChatOrderCommand command, CancellationToken cancellationToken);
}
