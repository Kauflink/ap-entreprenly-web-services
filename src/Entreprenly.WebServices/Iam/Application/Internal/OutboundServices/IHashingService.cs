namespace Entreprenly.WebServices.Iam.Application.Internal.OutboundServices;

/// <summary>
///     Hashes and verifies passwords.
/// </summary>
public interface IHashingService
{
    string HashPassword(string password);

    bool VerifyPassword(string password, string passwordHash);
}
