using Entreprenly.WebServices.Profiles.Domain.Model.Commands;
using Entreprenly.WebServices.Profiles.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Profiles.Interfaces.Rest.Transform;

public static class UpdatePreferencesCommandFromResourceAssembler
{
    public static UpdatePreferencesCommand ToCommandFromResource(int profileId, UpdatePreferencesResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdatePreferencesCommand(profileId, resource.Language, resource.Timezone, resource.Theme,
            resource.Currency);
    }
}
