using Entreprenly.WebServices.Iam.Application.CommandServices;
using Entreprenly.WebServices.Iam.Domain.Model.Commands;
using Entreprenly.WebServices.Iam.Domain.Model.Entities;
using Entreprenly.WebServices.Iam.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Iam.Domain.Repositories;
using Entreprenly.WebServices.Shared.Domain.Repositories;

namespace Entreprenly.WebServices.Iam.Application.Internal.CommandServices;

/// <summary>
///     Handles role commands, including seeding the system role catalog.
/// </summary>
public class RoleCommandService(IRoleRepository roleRepository, IUnitOfWork unitOfWork) : IRoleCommandService
{
    public async Task Handle(SeedRolesCommand command, CancellationToken cancellationToken)
    {
        foreach (var roleName in Enum.GetValues<ERoles>())
            if (!await roleRepository.ExistsByNameAsync(roleName, cancellationToken))
                await roleRepository.AddAsync(new Role(roleName), cancellationToken);

        await unitOfWork.CompleteAsync(cancellationToken);
    }
}
