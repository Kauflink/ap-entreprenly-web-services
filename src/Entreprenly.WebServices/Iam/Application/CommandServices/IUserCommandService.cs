using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Domain.Model.Commands;
using Entreprenly.WebServices.Shared.Application.Model;

namespace Entreprenly.WebServices.Iam.Application.CommandServices;

/// <summary>
///     Handles user-related commands.
/// </summary>
public interface IUserCommandService
{
    Task<Result<(User user, string token)>> Handle(SignInCommand command, CancellationToken cancellationToken);

    Task<Result> Handle(SignUpCommand command, CancellationToken cancellationToken);
}
