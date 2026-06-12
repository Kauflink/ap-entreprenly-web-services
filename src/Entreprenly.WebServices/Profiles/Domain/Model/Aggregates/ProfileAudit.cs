using Entreprenly.WebServices.Shared.Domain.Model.Entities;

namespace Entreprenly.WebServices.Profiles.Domain.Model.Aggregates;

/// <summary>
///     Audit partial for the <see cref="Profile" /> aggregate. Timestamps are populated automatically
///     by the persistence-layer auditing interceptor.
/// </summary>
public partial class Profile : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
