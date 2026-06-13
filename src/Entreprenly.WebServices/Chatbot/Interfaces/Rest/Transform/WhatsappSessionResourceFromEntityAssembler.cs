using Entreprenly.WebServices.Chatbot.Domain.Model.Aggregates;
using Entreprenly.WebServices.Chatbot.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Chatbot.Interfaces.Rest.Transform;

public static class WhatsappSessionResourceFromEntityAssembler
{
    public static WhatsappSessionResource ToResourceFromEntity(WhatsappSession session) =>
        new(session.Id, session.SellerId, session.OwnerEmail, session.BusinessName,
            session.Status.ToString(), session.Phone, session.ConnectedAt);
}
