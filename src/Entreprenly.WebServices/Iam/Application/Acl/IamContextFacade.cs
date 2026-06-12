using Entreprenly.WebServices.Iam.Application.QueryServices;
using Entreprenly.WebServices.Iam.Domain.Model.Queries;
using Entreprenly.WebServices.Iam.Interfaces.Acl;

namespace Entreprenly.WebServices.Iam.Application.Acl;

/// <summary>
///     Anti-corruption layer implementation exposing IAM data to other bounded contexts.
/// </summary>
public class IamContextFacade(IUserQueryService userQueryService) : IIamContextFacade
{
    public async Task<int> FetchUserIdByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await userQueryService.Handle(new GetUserByEmailQuery(email), cancellationToken);
        return user?.Id ?? 0;
    }

    public async Task<string> FetchEmailByUserId(int userId, CancellationToken cancellationToken)
    {
        var user = await userQueryService.Handle(new GetUserByIdQuery(userId), cancellationToken);
        return user?.Email ?? string.Empty;
    }
}
