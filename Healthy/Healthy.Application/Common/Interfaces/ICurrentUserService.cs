using Healthy.Application.Common.Models;

namespace Healthy.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    
    string? UserName { get; }
    
    string? Email { get; }
    
    string? FullName { get; }
    
    List<string> Roles { get; }
    
    bool IsAuthenticated { get; }
    
    CurrentUserInfo GetCurrentUserInfo();
    
    bool HasRole(string role);
    
    bool HasClaim(string claimType);
    
    string? GetClaimValue(string claimType);
} 