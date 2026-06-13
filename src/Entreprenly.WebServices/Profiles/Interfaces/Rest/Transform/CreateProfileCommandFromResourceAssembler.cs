using Entreprenly.WebServices.Profiles.Domain.Model.Commands;
using Entreprenly.WebServices.Profiles.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Profiles.Interfaces.Rest.Transform;

public static class CreateProfileCommandFromResourceAssembler
{
    public static CreateProfileCommand ToCommandFromResource(CreateProfileResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new CreateProfileCommand(
            resource.UserId,
            resource.FirstName,
            resource.LastName,
            resource.Role,
            resource.Plan,
            resource.Phone,
            resource.Timezone);
    }
}
