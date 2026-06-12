namespace Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Marks an action as not requiring authorization. The request authorization middleware
///     skips token validation for endpoints decorated with this attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AllowAnonymousAttribute : Attribute
{
}
