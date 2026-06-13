using Entreprenly.WebServices.Iam.Application.Internal.OutboundServices;
using Entreprenly.WebServices.Iam.Application.QueryServices;
using Entreprenly.WebServices.Iam.Domain.Model.Queries;
using Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Attributes;

namespace Entreprenly.WebServices.Iam.Infrastructure.Pipeline.Middleware.Components;

/// <summary>
///     Validates the bearer token on incoming requests. On success the resolved user is stored in
///     <c>HttpContext.Items["User"]</c> for downstream authorization.
/// </summary>
public class RequestAuthorizationMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IUserQueryService userQueryService,
        ITokenService tokenService)
    {
        var cancellationToken = context.RequestAborted;

        var allowAnonymous = context.Request.HttpContext.GetEndpoint()?.Metadata
            .Any(m => m.GetType() == typeof(AllowAnonymousAttribute)) ?? false;

        if (allowAnonymous)
        {
            await next(context);
            return;
        }

        var token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(" ").Last();

        var userId = await tokenService.ValidateToken(token ?? string.Empty);

        if (userId != null)
        {
            var user = await userQueryService.Handle(new GetUserByIdQuery(userId.Value), cancellationToken);
            context.Items["User"] = user;
        }

        await next(context);
    }
}
