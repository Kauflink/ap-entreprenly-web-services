using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;

namespace Entreprenly.WebServices.Iam.Application.Internal.OutboundServices;

/// <summary>
///     Generates and validates JWT tokens.
/// </summary>
public interface ITokenService
{
    string GenerateToken(User user);

    Task<int?> ValidateToken(string token);
}
