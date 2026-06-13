using Entreprenly.WebServices.Profiles.Domain.Model.Commands;
using Entreprenly.WebServices.Profiles.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Profiles.Interfaces.Rest.Transform;

public static class UpdateNotificationSettingsCommandFromResourceAssembler
{
    public static UpdateNotificationSettingsCommand ToCommandFromResource(int profileId,
        UpdateNotificationSettingsResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateNotificationSettingsCommand(profileId, resource.StockAlerts, resource.PaymentAlerts,
            resource.ChatbotMessages);
    }
}
