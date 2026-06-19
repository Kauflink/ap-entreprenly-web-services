using Cortex.Mediator;
using Entreprenly.WebServices.Iam.Application.CommandServices;
using Entreprenly.WebServices.Iam.Application.Internal.OutboundServices;
using Entreprenly.WebServices.Iam.Domain.Model;
using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Entreprenly.WebServices.Iam.Domain.Model.Commands;
using Entreprenly.WebServices.Iam.Domain.Model.Events;
using Entreprenly.WebServices.Iam.Domain.Model.ValueObjects;
using Entreprenly.WebServices.Iam.Domain.Repositories;
using Entreprenly.WebServices.Resources.Errors;
using Entreprenly.WebServices.Shared.Application.Model;
using Entreprenly.WebServices.Shared.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace Entreprenly.WebServices.Iam.Application.Internal.CommandServices;

/// <summary>
///     Handles user commands: authentication and registration.
/// </summary>
public class UserCommandService(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ITokenService tokenService,
    IHashingService hashingService,
    IUnitOfWork unitOfWork,
    IMediator mediator,
    IStringLocalizer<ErrorMessages> localizer)
    : IUserCommandService
{
    public async Task<Result<(User user, string token)>> Handle(SignInCommand command,
        CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByEmailAsync(command.Email, cancellationToken);

        if (user is null)
            return Result<(User user, string token)>.Failure(IamError.UserNotFound,
                localizer[nameof(IamError.UserNotFound)]);

        if (!hashingService.VerifyPassword(command.Password, user.PasswordHash))
            return Result<(User user, string token)>.Failure(IamError.InvalidCredentials,
                localizer[nameof(IamError.InvalidCredentials)]);

        var token = tokenService.GenerateToken(user);

        return Result<(User user, string token)>.Success((user, token));
    }

    public async Task<Result<User>> Handle(SignUpCommand command, CancellationToken cancellationToken)
    {
        if (await userRepository.ExistsByEmailAsync(command.Email, cancellationToken))
            return Result<User>.Failure(IamError.EmailAlreadyTaken,
                localizer[nameof(IamError.EmailAlreadyTaken), command.Email]);

        var defaultRole = await roleRepository.FindByNameAsync(ERoles.ROLE_USER, cancellationToken);
        if (defaultRole is null)
            return Result<User>.Failure(IamError.RoleNotFound, localizer[nameof(IamError.RoleNotFound)]);

        var hashedPassword = hashingService.HashPassword(command.Password);
        var user = new User(command.Email, hashedPassword, [defaultRole]);

        try
        {
            await userRepository.AddAsync(user, cancellationToken);
            await unitOfWork.CompleteAsync(cancellationToken);

            await mediator.PublishAsync(
                new UserSignedUpEvent(user.Id, user.Email, command.FirstName, command.LastName, command.Phone,
                    command.Timezone),
                cancellationToken);

            return Result<User>.Success(user);
        }
        catch (OperationCanceledException)
        {
            return Result<User>.Failure(IamError.OperationCancelled, localizer[nameof(IamError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<User>.Failure(IamError.DatabaseError, localizer[nameof(IamError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<User>.Failure(IamError.InternalServerError, localizer[nameof(IamError.InternalServerError)]);
        }
    }

    public async Task<Result<User>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);

        if (user is null)
            return Result<User>.Failure(IamError.UserNotFound, localizer[nameof(IamError.UserNotFound)]);

        if (!hashingService.VerifyPassword(command.CurrentPassword, user.PasswordHash))
            return Result<User>.Failure(IamError.CurrentPasswordIncorrect,
                localizer[nameof(IamError.CurrentPasswordIncorrect)]);

        user.UpdatePasswordHash(hashingService.HashPassword(command.NewPassword));

        try
        {
            userRepository.Update(user);
            await unitOfWork.CompleteAsync(cancellationToken);

            return Result<User>.Success(user);
        }
        catch (OperationCanceledException)
        {
            return Result<User>.Failure(IamError.OperationCancelled, localizer[nameof(IamError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<User>.Failure(IamError.DatabaseError, localizer[nameof(IamError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<User>.Failure(IamError.InternalServerError, localizer[nameof(IamError.InternalServerError)]);
        }
    }

    public async Task<Result<User>> Handle(ChangeEmailCommand command, CancellationToken cancellationToken)
    {
        var user = await userRepository.FindByIdAsync(command.UserId, cancellationToken);

        if (user is null)
            return Result<User>.Failure(IamError.UserNotFound, localizer[nameof(IamError.UserNotFound)]);

        if (string.Equals(command.NewEmail, user.Email, StringComparison.OrdinalIgnoreCase))
            return Result<User>.Failure(IamError.EmailMatchesCurrent,
                localizer[nameof(IamError.EmailMatchesCurrent)]);

        if (await userRepository.ExistsByEmailAsync(command.NewEmail, cancellationToken))
            return Result<User>.Failure(IamError.EmailAlreadyTaken,
                localizer[nameof(IamError.EmailAlreadyTaken), command.NewEmail]);

        user.UpdateEmail(command.NewEmail);

        try
        {
            userRepository.Update(user);
            await unitOfWork.CompleteAsync(cancellationToken);

            return Result<User>.Success(user);
        }
        catch (OperationCanceledException)
        {
            return Result<User>.Failure(IamError.OperationCancelled, localizer[nameof(IamError.OperationCancelled)]);
        }
        catch (DbUpdateException)
        {
            return Result<User>.Failure(IamError.DatabaseError, localizer[nameof(IamError.DatabaseError)]);
        }
        catch (Exception)
        {
            return Result<User>.Failure(IamError.InternalServerError, localizer[nameof(IamError.InternalServerError)]);
        }
    }
}
