using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using Healthy.Infrastructure.Constants;

namespace Healthy.Infrastructure.Authorization;

/// <summary>
/// Custom authorization attribute for role-based access control
/// Can be applied to controllers or specific action methods
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class AuthorizeRolesAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _roles;
    private readonly bool _requireAllRoles;

    /// <summary>
    /// Initialize with required roles
    /// </summary>
    /// <param name="roles">Required roles for access</param>
    public AuthorizeRolesAttribute(params string[] roles)
    {
        _roles = roles ?? throw new ArgumentNullException(nameof(roles));
        _requireAllRoles = false;
    }

    /// <summary>
    /// Initialize with required roles and specify if all roles are required
    /// </summary>
    /// <param name="requireAllRoles">If true, user must have ALL specified roles. If false, user needs ANY of the roles</param>
    /// <param name="roles">Required roles for access</param>
    public AuthorizeRolesAttribute(bool requireAllRoles, params string[] roles)
    {
        _roles = roles ?? throw new ArgumentNullException(nameof(roles));
        _requireAllRoles = requireAllRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // Check if user is authenticated
        if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
        {
            context.Result = new UnauthorizedResult();
            return;
        }

        // Get user roles from claims
        var userRoles = context.HttpContext.User
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList();

        // Check if user has required roles
        bool hasAccess = _requireAllRoles 
            ? _roles.All(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase))
            : _roles.Any(role => userRoles.Contains(role, StringComparer.OrdinalIgnoreCase));

        if (!hasAccess)
        {
            context.Result = new ForbidResult();
            return;
        }
    }
}

/// <summary>
/// Authorization attribute for Admin role only
/// </summary>
public class AdminOnlyAttribute : AuthorizeRolesAttribute
{
    public AdminOnlyAttribute() : base(Roles.Admin) { }
}

/// <summary>
/// Authorization attribute for User and Admin roles
/// </summary>
public class UserOrAdminAttribute : AuthorizeRolesAttribute
{
    public UserOrAdminAttribute() : base(Roles.User, Roles.Admin) { }
}

/// <summary>
/// Authorization attribute for Moderator and Admin roles
/// </summary>
public class ModeratorOrAdminAttribute : AuthorizeRolesAttribute
{
    public ModeratorOrAdminAttribute() : base(Roles.Moderator, Roles.Admin) { }
}

/// <summary>
/// Authorization attribute that requires ALL specified roles
/// </summary>
public class RequireAllRolesAttribute : AuthorizeRolesAttribute
{
    public RequireAllRolesAttribute(params string[] roles) : base(true, roles) { }
}
