using Entreprenly.WebServices.Profiles.Domain.Model.Aggregates;
using Entreprenly.WebServices.Profiles.Domain.Model.Queries;

namespace Entreprenly.WebServices.Profiles.Application.QueryServices;

/// <summary>
///     Handles profile-related queries.
/// </summary>
public interface IProfileQueryService
{
    Task<IEnumerable<Profile>> Handle(GetAllProfilesQuery query, CancellationToken cancellationToken);

    Task<Profile?> Handle(GetProfileByIdQuery query, CancellationToken cancellationToken);

    Task<Profile?> Handle(GetProfileByUserIdQuery query, CancellationToken cancellationToken);
}
