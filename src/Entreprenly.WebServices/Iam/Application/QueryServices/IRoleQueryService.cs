using Entreprenly.WebServices.Iam.Domain.Model.Entities;
using Entreprenly.WebServices.Iam.Domain.Model.Queries;

namespace Entreprenly.WebServices.Iam.Application.QueryServices;

/// <summary>
///     Handles role-related queries.
/// </summary>
public interface IRoleQueryService
{
    Task<IEnumerable<Role>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken);

    Task<Role?> Handle(GetRoleByNameQuery query, CancellationToken cancellationToken);
}
