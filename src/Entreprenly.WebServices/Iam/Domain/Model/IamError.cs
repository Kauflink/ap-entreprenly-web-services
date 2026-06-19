namespace Entreprenly.WebServices.Iam.Domain.Model;

public enum IamError
{
    None,
    UserNotFound,
    EmailAlreadyTaken,
    InvalidCredentials,
    CurrentPasswordIncorrect,
    EmailMatchesCurrent,
    RoleNotFound,
    OperationCancelled,
    DatabaseError,
    InternalServerError,
    ExternalServiceError
}
