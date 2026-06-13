using Entreprenly.WebServices.Shared.Domain.Model.Entities;

namespace Entreprenly.WebServices.Iam.Domain.Model.Aggregates;

/// <summary>
///     Audit partial for the <see cref="User" /> aggregate. Timestamps are populated automatically
///     by the persistence-layer auditing interceptor.
/// </summary>
public partial class User : IAuditableEntity
{
    public DateTimeOffset? CreatedAt { get; set; }
    public DateTimeOffset? UpdatedAt { get; set; }
}
