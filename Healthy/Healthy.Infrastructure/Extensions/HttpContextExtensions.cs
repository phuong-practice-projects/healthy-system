using Microsoft.AspNetCore.Http;

namespace Healthy.Infrastructure.Extensions;

public static class HttpContextExtensions
{
    /// <summary>
    /// Get user ID from HttpContext.Items (populated by UserAuthorizationMiddleware)
    /// </summary>
    public static string? GetUserId(this HttpContext context)
    {
        return context.Items["UserId"]?.ToString();
    }

    /// <summary>
    /// Get user name from HttpContext.Items (populated by UserAuthorizationMiddleware)
    /// </summary>
    public static string? GetUserName(this HttpContext context)
    {
        return context.Items["UserName"]?.ToString();
    }

    /// <summary>
    /// Get user email from HttpContext.Items (populated by UserAuthorizationMiddleware)
    /// </summary>
    public static string? GetUserEmail(this HttpContext context)
    {
        return context.Items["UserEmail"]?.ToString();
    }

    /// <summary>
    /// Get user roles from HttpContext.Items (populated by UserAuthorizationMiddleware)
    /// </summary>
    public static string[] GetUserRoles(this HttpContext context)
    {
        return context.Items["UserRoles"] as string[] ?? Array.Empty<string>();
    }

    /// <summary>
    /// Check if user is authenticated from HttpContext.Items (populated by UserAuthorizationMiddleware)
    /// </summary>
    public static bool IsUserAuthenticated(this HttpContext context)
    {
        return context.Items["IsAuthenticated"] as bool? ?? false;
    }

    /// <summary>
    /// Check if user has specific role from HttpContext.Items (populated by UserAuthorizationMiddleware)
    /// </summary>
    public static bool HasUserRole(this HttpContext context, string role)
    {
        var roles = context.GetUserRoles();
        return roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Get user ID as Guid from HttpContext.Items (populated by UserAuthorizationMiddleware)
    /// </summary>
    public static Guid GetUserIdAsGuid(this HttpContext context)
    {
        var userId = context.GetUserId();
        return Guid.TryParse(userId, out var guid) ? guid : Guid.Empty;
    }
}
