using Entreprenly.WebServices.Iam.Domain.Model.Entities;
using Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Transform;

public static class RoleResourceFromEntityAssembler
{
    public static RoleResource ToResourceFromEntity(Role role)
    {
        ArgumentNullException.ThrowIfNull(role);
        return new RoleResource(role.Id, role.StringName);
    }
}
