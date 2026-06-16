using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;

namespace Entreprenly.WebServices.Chatbot.Application.QueryServices;

public interface IWhatsappSessionQueryService
{
    Task<IEnumerable<WhatsappSession>> Handle(GetAllWhatsappSessionsQuery query, CancellationToken cancellationToken);
    Task<WhatsappSession?> Handle(GetWhatsappSessionBySellerIdQuery query, CancellationToken cancellationToken);
}
