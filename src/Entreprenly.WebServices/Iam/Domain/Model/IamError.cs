namespace Entreprenly.WebServices.Iam.Domain.Model;

public enum IamError
{
    None,
    UserNotFound,
    EmailAlreadyTaken,
    InvalidCredentials,
    RoleNotFound,
    OperationCancelled,
    DatabaseError,
    InternalServerError,
    ExternalServiceError
}
