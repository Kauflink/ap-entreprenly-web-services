using Entreprenly.WebServices.Profiles.Domain.Model.Commands;
using Entreprenly.WebServices.Profiles.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Profiles.Interfaces.Rest.Transform;

public static class UpdateProfileCommandFromResourceAssembler
{
    public static UpdateProfileCommand ToCommandFromResource(int profileId, UpdateProfileResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new UpdateProfileCommand(profileId, resource.FirstName, resource.LastName, resource.Phone,
            resource.AvatarUrl);
    }
}
