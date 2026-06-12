using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Interfaces.Rest.Resources;

namespace Entreprenly.WebServices.Iam.Interfaces.Rest.Transform;

public static class AuthenticatedUserResourceFromEntityAssembler
{
    public static AuthenticatedUserResource ToResourceFromEntity(User user, string token)
    {
        ArgumentNullException.ThrowIfNull(user);
        if (string.IsNullOrEmpty(token))
            throw new ArgumentException("Token cannot be null or empty.", nameof(token));
        return new AuthenticatedUserResource(user.Id, user.Email, token);
    }
}
