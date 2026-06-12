namespace Entreprenly.WebServices.Profiles.Domain.Model;

public enum ProfilesError
{
    None,
    ProfileNotFound,
    ProfileAlreadyExists,
    InvalidProfileData,
    OperationCancelled,
    DatabaseError,
    InternalServerError
}
