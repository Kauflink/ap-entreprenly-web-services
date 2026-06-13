using Entreprenly.WebServices.Iam.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;

/// <summary>
///     Requires an authenticated user. Returns 401 when no user has been set on the request by the
///     request authorization middleware.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous) return;

        var user = (User?)context.HttpContext.Items["User"];
        if (user == null) context.Result = new UnauthorizedResult();
    }
}
