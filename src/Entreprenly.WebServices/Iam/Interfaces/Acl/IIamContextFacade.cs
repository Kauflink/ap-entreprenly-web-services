namespace Entreprenly.WebServices.Iam.Interfaces.Acl;

/// <summary>
///     Anti-corruption layer exposing IAM data to other bounded contexts.
/// </summary>
public interface IIamContextFacade
{
    Task<int> FetchUserIdByEmail(string email, CancellationToken cancellationToken);

    Task<string> FetchEmailByUserId(int userId, CancellationToken cancellationToken);
}
