using Entreprenly.WebServices.Iam.Domain.Model.Commands;

namespace Entreprenly.WebServices.Iam.Application.CommandServices;

/// <summary>
///     Handles role-related commands.
/// </summary>
public interface IRoleCommandService
{
    Task Handle(SeedRolesCommand command, CancellationToken cancellationToken);
}
