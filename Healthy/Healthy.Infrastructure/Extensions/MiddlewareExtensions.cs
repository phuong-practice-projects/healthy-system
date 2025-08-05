using Healthy.Infrastructure.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Healthy.Infrastructure.Extensions;

public static class MiddlewareExtensions
{
    /// <summary>
    /// Adds global exception handling middleware
    /// </summary>
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }

    /// <summary>
    /// Adds User Authorization middleware to extract JWT token, validate it, 
    /// populate user context in HttpContext.Items, and setup CurrentUserService
    /// </summary>
    public static IApplicationBuilder UseUserAuthorization(this IApplicationBuilder app)
    {
        return app.UseMiddleware<UserAuthorizationMiddleware>();
    }

    /// <summary>
    /// Adds complete authentication stack
    /// </summary>
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        return app.UseUserAuthorization();    // Extract and validate JWT, populate user context
    }
}
