// ReSharper disable InconsistentNaming

namespace Entreprenly.WebServices.Iam.Domain.Model.ValueObjects;

/// <summary>
///     Enumerates the roles a user can hold within the system. The member names are the
///     persisted role identifiers and the published contract consumed by clients.
/// </summary>
public enum ERoles
{
    ROLE_USER,
    ROLE_ADMIN
}
