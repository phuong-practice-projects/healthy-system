using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Healthy.Infrastructure.Extensions;
using Healthy.Infrastructure.Constants;

namespace Healthy.Infrastructure.Authorization;

/// <summary>
/// Authorization attribute to ensure user can only access their own resources
/// or allow admin to access any resource
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class AuthorizeOwnerOrAdminAttribute : Attribute, IAuthorizationFilter
{
    private readonly string _userIdParameterName;

    /// <summary>
    /// Initialize with parameter name that contains the user ID
    /// </summary>
    /// <param name="userIdParameterName">Name of the route/query parameter containing user ID (default: "userId")</param>
    public AuthorizeOwnerOrAdminAttribute(string userIdParameterName = "userId")
    {
        _userIdParameterName = userIdParameterName;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get current user ID from claims
        var currentUserIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(currentUserIdClaim))
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Check if user is admin (admin can access any resource)
        var userRoles = context.HttpContext.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        if (userRoles.Contains(Roles.Admin, StringComparer.OrdinalIgnoreCase))
        {
            return; // Admin has access to everything
        }

        // Get the target user ID from route/query parameters
        var routeUserId = context.RouteData.Values[_userIdParameterName]?.ToString() ??
                         context.HttpContext.Request.Query[_userIdParameterName].FirstOrDefault();

        if (string.IsNullOrEmpty(routeUserId))
        {
            context.Result = new BadRequestObjectResult($"Missing {_userIdParameterName} parameter");
            return;
        }

        // Compare user IDs using GUID comparison (case-insensitive)
        var currentUserId = currentUserIdClaim.ToGuid();
        var targetUserId = routeUserId.ToGuid();

        if (currentUserId == Guid.Empty || targetUserId == Guid.Empty)
        {
            context.Result = new BadRequestObjectResult("Invalid user ID format");
            return;
        }

        if (currentUserId != targetUserId)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}

/// <summary>
/// Authorization attribute for development/testing purposes only
/// Should be removed in production
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class DevOnlyAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        if (!string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
        {
            context.Result = new NotFoundResult();
        }
    }
}
