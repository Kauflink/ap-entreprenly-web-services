using Entreprenly.WebServices.Iam.Application.Internal.OutboundServices;
using BCryptNet = BCrypt.Net.BCrypt;

namespace Entreprenly.WebServices.Iam.Infrastructure.Hashing.BCrypt.Services;

/// <summary>
///     BCrypt-based password hashing and verification.
/// </summary>
public class HashingService : IHashingService
{
    public string HashPassword(string password)
    {
        return BCryptNet.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCryptNet.Verify(password, passwordHash);
    }
}
