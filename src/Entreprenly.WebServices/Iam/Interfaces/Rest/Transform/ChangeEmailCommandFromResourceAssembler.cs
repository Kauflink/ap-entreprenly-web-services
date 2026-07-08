using Entreprenly.WebServices.Iam.Domain.Model.Commands;
using Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Transform;

public static class ChangeEmailCommandFromResourceAssembler
{
    public static ChangeEmailCommand ToCommandFromResource(int userId, ChangeEmailResource resource)
    {
        return new ChangeEmailCommand(userId, resource.Email);
    }
}
