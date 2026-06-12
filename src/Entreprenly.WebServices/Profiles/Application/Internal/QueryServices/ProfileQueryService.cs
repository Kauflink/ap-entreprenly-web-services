using Entreprenly.WebServices.Profiles.Application.QueryServices;
using Entreprenly.WebServices.Profiles.Domain.Model.Aggregates;
using Entreprenly.WebServices.Profiles.Domain.Model.Queries;
using Entreprenly.WebServices.Profiles.Domain.Repositories;

namespace Entreprenly.WebServices.Profiles.Application.Internal.QueryServices;

/// <summary>
///     Handles profile queries.
/// </summary>
public class ProfileQueryService(IProfileRepository profileRepository) : IProfileQueryService
{
    public async Task<IEnumerable<Profile>> Handle(GetAllProfilesQuery query, CancellationToken cancellationToken)
    {
        return await profileRepository.ListAsync(cancellationToken);
    }

    public async Task<Profile?> Handle(GetProfileByIdQuery query, CancellationToken cancellationToken)
    {
        return await profileRepository.FindByIdAsync(query.ProfileId, cancellationToken);
    }

    public async Task<Profile?> Handle(GetProfileByUserIdQuery query, CancellationToken cancellationToken)
    {
        return await profileRepository.FindByUserIdAsync(query.UserId, cancellationToken);
    }
}
