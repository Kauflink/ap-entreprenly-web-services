using Entreprenly.WebServices.Iam.Domain.Model.Commands;
using Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Transform;

public static class ChangePasswordCommandFromResourceAssembler
{
    public static ChangePasswordCommand ToCommandFromResource(int userId, ChangePasswordResource resource)
    {
        return new ChangePasswordCommand(userId, resource.CurrentPassword, resource.NewPassword);
    }
}
