using Entreprenly.WebServices.Shared.Domain.Model;

namespace Entreprenly.WebServices.Profiles.Domain.Model.Errors;

/// <summary>
///     Catalog of Profiles domain errors, exposed as reusable <see cref="Error" /> instances.
/// </summary>
public static class ProfilesErrors
{
    public static readonly Error ProfileNotFound =
        new("Profiles.ProfileNotFound", "Profile not found.");

    public static readonly Error ProfileAlreadyExists =
        new("Profiles.ProfileAlreadyExists", "A profile already exists for this user.");

    public static readonly Error InvalidProfileData =
        new("Profiles.InvalidProfileData", "Invalid profile data provided.");

    public static readonly Error OperationCancelled =
        new("Profiles.OperationCancelled", "The operation was cancelled.");

    public static readonly Error DatabaseError =
        new("Profiles.DatabaseError", "A database error occurred.");

    public static readonly Error InternalServerError =
        new("Profiles.InternalServerError", "An unexpected internal error occurred.");
}
