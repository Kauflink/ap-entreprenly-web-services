using Entreprenly.WebServices.Iam.Domain.Model.Commands;
using Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Transform;

public static class SignUpCommandFromResourceAssembler
{
    public static SignUpCommand ToCommandFromResource(SignUpResource resource)
    {
        ArgumentNullException.ThrowIfNull(resource);
        return new SignUpCommand(
            resource.Email,
            resource.Password,
            resource.FirstName,
            resource.LastName,
            resource.Phone,
            resource.Timezone);
    }
}
