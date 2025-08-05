using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Healthy.Infrastructure.Constants;

namespace Healthy.Infrastructure.Authorization;

/// <summary>
/// Authorization policies for the application
/// </summary>
public static class AuthorizationPolicies
{
    public const string AdminOnly = "AdminOnly";
    public const string UserOrAdmin = "UserOrAdmin";
    public const string ModeratorOrAdmin = "ModeratorOrAdmin";
    public const string RequireAuthenticated = "RequireAuthenticated";

    /// <summary>
    /// Configure authorization policies
    /// </summary>
    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        // Admin only policy
        options.AddPolicy(AdminOnly, policy =>
            policy.RequireRole(Roles.Admin));

        // User or Admin policy
        options.AddPolicy(UserOrAdmin, policy =>
            policy.RequireRole(Roles.User, Roles.Admin));

        // Moderator or Admin policy
        options.AddPolicy(ModeratorOrAdmin, policy =>
            policy.RequireRole(Roles.Moderator, Roles.Admin));

        // Require authenticated user
        options.AddPolicy(RequireAuthenticated, policy =>
            policy.RequireAuthenticatedUser());
    }
}

/// <summary>
/// Custom authorization requirements
/// </summary>
public class OwnerOrAdminRequirement : IAuthorizationRequirement
{
    public string UserIdParameterName { get; }

    public OwnerOrAdminRequirement(string userIdParameterName = "userId")
    {
        UserIdParameterName = userIdParameterName;
    }
}

/// <summary>
/// Handler for OwnerOrAdmin authorization requirement
/// </summary>
public class OwnerOrAdminHandler : AuthorizationHandler<OwnerOrAdminRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OwnerOrAdminRequirement requirement)
    {
        // This handler is primarily for demonstration
        // The main logic should be in AuthorizeOwnerOrAdminAttribute
        // which has direct access to route data
        
        if (!context.User.Identity?.IsAuthenticated ?? true)
        {
            return Task.CompletedTask;
        }

        // Check if user is admin
        if (context.User.IsInRole(Roles.Admin))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
