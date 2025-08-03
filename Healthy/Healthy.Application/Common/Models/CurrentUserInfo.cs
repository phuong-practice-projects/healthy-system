namespace Healthy.Application.Common.Models;

public class CurrentUserInfo
{
    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? FullName { get; set; }
    public List<string> Roles { get; set; } = new();
    public Dictionary<string, string> Claims { get; set; } = new();
    public bool IsAuthenticated { get; set; }
    public DateTime? TokenExpiration { get; set; }
    
    public bool HasRole(string role)
    {
        return Roles.Contains(role, StringComparer.OrdinalIgnoreCase);
    }
    
    public bool HasClaim(string claimType)
    {
        return Claims.ContainsKey(claimType);
    }
    
    public string? GetClaimValue(string claimType)
    {
        return Claims.TryGetValue(claimType, out var value) ? value : null;
    }
}
