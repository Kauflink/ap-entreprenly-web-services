using Entreprenly.WebServices.Iam.Application.QueryServices;
using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Domain.Model.Queries;
using Entreprenly.WebServices.Iam.Domain.Repositories;

namespace Entreprenly.WebServices.Iam.Application.Internal.QueryServices;

/// <summary>
///     Handles user queries.
/// </summary>
public class UserQueryService(IUserRepository userRepository) : IUserQueryService
{
    public async Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.FindByIdAsync(query.Id, cancellationToken);
    }

    public async Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.ListAsync(cancellationToken);
    }

    public async Task<User?> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        return await userRepository.FindByEmailAsync(query.Email, cancellationToken);
    }
}
