using Entreprenly.WebServices.Iam.Application.QueryServices;
using Entreprenly.WebServices.Iam.Domain.Model.Entities;
using Entreprenly.WebServices.Iam.Domain.Model.Queries;
using Entreprenly.WebServices.Iam.Domain.Repositories;

namespace Entreprenly.WebServices.Iam.Application.Internal.QueryServices;

/// <summary>
///     Handles role queries.
/// </summary>
public class RoleQueryService(IRoleRepository roleRepository) : IRoleQueryService
{
    public async Task<IEnumerable<Role>> Handle(GetAllRolesQuery query, CancellationToken cancellationToken)
    {
        return await roleRepository.ListAsync(cancellationToken);
    }

    public async Task<Role?> Handle(GetRoleByNameQuery query, CancellationToken cancellationToken)
    {
        return await roleRepository.FindByNameAsync(query.Name, cancellationToken);
    }
}
