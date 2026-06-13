using Entreprenly.WebServices.Shared.Infrastructure.Pipeline.Middleware.Components;

namespace Entreprenly.WebServices.Shared.Infrastructure.Pipeline.Middleware.Extensions;

/// <summary>
///     Middleware registration extensions.
/// </summary>
public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
    }
}
