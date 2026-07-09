using Entreprenly.WebServices.Shared.Domain.Model;

namespace Entreprenly.WebServices.Iam.Domain.Model.Errors;

/// <summary>
///     Catalog of IAM domain errors, exposed as reusable <see cref="Error" /> instances.
/// </summary>
public static class IamErrors
{
    public static readonly Error UserNotFound =
        new("Iam.UserNotFound", "User not found.");

    public static readonly Error EmailAlreadyTaken =
        new("Iam.EmailAlreadyTaken", "The email address is already registered.");

    public static readonly Error InvalidCredentials =
        new("Iam.InvalidCredentials", "Invalid email or password.");

    public static readonly Error CurrentPasswordIncorrect =
        new("Iam.CurrentPasswordIncorrect", "The current password is incorrect.");

    public static readonly Error EmailMatchesCurrent =
        new("Iam.EmailMatchesCurrent", "The new email matches the current email.");

    public static readonly Error RoleNotFound =
        new("Iam.RoleNotFound", "The requested role was not found.");

    public static readonly Error OperationCancelled =
        new("Iam.OperationCancelled", "The operation was cancelled.");

    public static readonly Error DatabaseError =
        new("Iam.DatabaseError", "A database error occurred.");

    public static readonly Error InternalServerError =
        new("Iam.InternalServerError", "An unexpected internal error occurred.");

    public static readonly Error ExternalServiceError =
        new("Iam.ExternalServiceError", "An external service error occurred.");
}
