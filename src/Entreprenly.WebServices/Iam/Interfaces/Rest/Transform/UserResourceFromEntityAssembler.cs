using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Transform;

public static class UserResourceFromEntityAssembler
{
    public static UserResource ToResourceFromEntity(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        return new UserResource(user.Id, user.Email, user.Roles.Select(role => role.StringName));
    }
}
