namespace Entreprenly.WebServices.Iam.Infrastructure.Tokens.Jwt.Configuration;

/// <summary>
///     JWT token settings bound from the application configuration.
/// </summary>
public class TokenSettings
{
    public required string Secret { get; set; }
}
