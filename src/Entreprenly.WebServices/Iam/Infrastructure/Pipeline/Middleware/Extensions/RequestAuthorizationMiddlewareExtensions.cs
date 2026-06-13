using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Components;

namespace Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Extensions;

/// <summary>
///     Registers the request authorization middleware in the ASP.NET Core pipeline.
/// </summary>
public static class RequestAuthorizationMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestAuthorization(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<RequestAuthorizationMiddleware>();
    }
}
