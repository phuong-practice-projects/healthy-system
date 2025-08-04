namespace Healthy.Infrastructure.Constants;

/// <summary>
/// Contains predefined role constants used throughout the application
/// Use these constants instead of hardcoded strings to ensure consistency
/// </summary>
public static class Roles
{
    /// <summary>
    /// Administrator role with full system access
    /// </summary>
    public const string Admin = "Admin";

    /// <summary>
    /// Regular user role with basic access
    /// </summary>
    public const string User = "User";

    /// <summary>
    /// Moderator role with limited admin access
    /// </summary>
    public const string Moderator = "Moderator";

    /// <summary>
    /// Manager role for business operations
    /// </summary>
    public const string Manager = "Manager";

    /// <summary>
    /// Super user role for system operations
    /// </summary>
    public const string SuperUser = "SuperUser";

    /// <summary>
    /// Verified user role for additional privileges
    /// </summary>
    public const string Verified = "Verified";

    /// <summary>
    /// Premium user role for premium features
    /// </summary>
    public const string Premium = "Premium";

    /// <summary>
    /// Get all available roles as an array
    /// </summary>
    public static readonly string[] AllRoles = 
    {
        Admin,
        User,
        Moderator,
        Manager,
        SuperUser,
        Verified,
        Premium
    };

    /// <summary>
    /// Get admin-level roles (roles with high privileges)
    /// </summary>
    public static readonly string[] AdminLevelRoles = 
    {
        Admin,
        SuperUser
    };

    /// <summary>
    /// Get standard user roles
    /// </summary>
    public static readonly string[] StandardUserRoles = 
    {
        User,
        Verified,
        Premium
    };

    /// <summary>
    /// Get moderation roles
    /// </summary>
    public static readonly string[] ModerationRoles = 
    {
        Admin,
        Moderator,
        Manager
    };
}
