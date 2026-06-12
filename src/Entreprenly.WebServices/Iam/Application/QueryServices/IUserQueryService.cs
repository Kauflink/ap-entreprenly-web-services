using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Domain.Model.Queries;

namespace Entreprenly.WebServices.Iam.Application.QueryServices;

/// <summary>
///     Handles user-related queries.
/// </summary>
public interface IUserQueryService
{
    Task<User?> Handle(GetUserByIdQuery query, CancellationToken cancellationToken);

    Task<IEnumerable<User>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken);

    Task<User?> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken);
}
