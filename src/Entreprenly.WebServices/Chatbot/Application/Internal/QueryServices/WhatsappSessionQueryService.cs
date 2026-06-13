using Entreprenly.WebServices.Chatbot.Application.QueryServices;
using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Domain.Model.Queries;
using Entreprenly.WebServices.Chatbot.Domain.Repositories;

namespace Entreprenly.WebServices.Chatbot.Application.Internal.QueryServices;

public class WhatsappSessionQueryService(IWhatsappSessionRepository whatsappSessionRepository)
    : IWhatsappSessionQueryService
{
    public async Task<IEnumerable<WhatsappSession>> Handle(GetAllWhatsappSessionsQuery query,
        CancellationToken cancellationToken)
    {
        return await whatsappSessionRepository.ListAsync(cancellationToken);
    }

    public async Task<WhatsappSession?> Handle(GetWhatsappSessionBySellerIdQuery query,
        CancellationToken cancellationToken)
    {
        return await whatsappSessionRepository.FindBySellerIdAsync(query.SellerId, cancellationToken);
    }
}
