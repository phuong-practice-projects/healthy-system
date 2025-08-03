using Healthy.Application.Common.Interfaces;
using Healthy.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Healthy.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<CurrentUserService> _logger;
    private CurrentUserInfo? _currentUserInfo;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor, ILogger<CurrentUserService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    public string? UserId => GetCurrentUserInfo().UserId;
    
    public string? UserName => GetCurrentUserInfo().UserName;
    
    public string? Email => GetCurrentUserInfo().Email;
    
    public string? FullName => GetCurrentUserInfo().FullName;
    
    public List<string> Roles => GetCurrentUserInfo().Roles;
    
    public bool IsAuthenticated => GetCurrentUserInfo().IsAuthenticated;

    public CurrentUserInfo GetCurrentUserInfo()
    {
        // Use lazy loading - compute once per request scope
        if (_currentUserInfo != null)
            return _currentUserInfo;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            _logger.LogDebug("HttpContext is null");
            _currentUserInfo = new CurrentUserInfo { IsAuthenticated = false };
            return _currentUserInfo;
        }

        // First, try to get pre-built CurrentUserInfo from HttpContext.Items (populated by UserAuthorizationMiddleware)
        if (httpContext.Items.TryGetValue("CurrentUserInfo", out var userInfoObj) && userInfoObj is CurrentUserInfo userInfo)
        {
            _logger.LogInformation("Using pre-built CurrentUserInfo from HttpContext.Items - UserId: {UserId}, UserName: {UserName}", 
                userInfo.UserId, userInfo.UserName);
            
            _currentUserInfo = userInfo;
            return _currentUserInfo;
        }

        // Fallback: try to get from HttpContext.Items (backward compatibility)
        var isAuthFromItems = httpContext.Items["IsAuthenticated"] as bool? ?? false;
        
        _logger.LogDebug("Checking authentication - Items: {ItemsAuth}, Items count: {ItemsCount}", 
            isAuthFromItems, httpContext.Items.Count);
        
        if (isAuthFromItems)
        {
            var userId = httpContext.Items["UserId"]?.ToString();
            var userName = httpContext.Items["UserName"]?.ToString();
            
            _logger.LogInformation("Using HttpContext.Items - UserId: {UserId}, UserName: {UserName}", 
                userId, userName);
            
            _currentUserInfo = new CurrentUserInfo
            {
                UserId = userId,
                UserName = userName,
                Email = httpContext.Items["UserEmail"]?.ToString(),
                FullName = httpContext.Items["UserFullName"]?.ToString() ?? userName,
                IsAuthenticated = true,
                Roles = (httpContext.Items["UserRoles"] as string[])?.ToList() ?? new List<string>(),
                Claims = new Dictionary<string, string>()
            };
            
            return _currentUserInfo;
        }

        // Final fallback to HttpContext.User (built-in authentication)
        var user = httpContext.User;
        
        _logger.LogDebug("Fallback to HttpContext.User - IsAuthenticated: {IsAuth}", 
            user?.Identity?.IsAuthenticated);
        
        if (user?.Identity?.IsAuthenticated != true)
        {
            _logger.LogDebug("User not authenticated");
            _currentUserInfo = new CurrentUserInfo
            {
                IsAuthenticated = false
            };
            return _currentUserInfo;
        }

        var userInfo2 = new CurrentUserInfo
        {
            UserId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value,
            UserName = user.FindFirst(ClaimTypes.Name)?.Value,
            Email = user.FindFirst(ClaimTypes.Email)?.Value,
            FullName = user.FindFirst("FullName")?.Value ?? 
                      user.FindFirst(ClaimTypes.GivenName)?.Value ??
                      user.FindFirst("name")?.Value,
            IsAuthenticated = true,
            Roles = user.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList(),
            Claims = user.Claims.ToDictionary(c => c.Type, c => c.Value)
        };

        // Try to get token expiration
        var expClaim = user.FindFirst("exp");
        if (expClaim != null && long.TryParse(expClaim.Value, out var exp))
        {
            userInfo2.TokenExpiration = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
        }

        _currentUserInfo = userInfo2;
        return _currentUserInfo;
    }

    public bool HasRole(string role)
    {
        return GetCurrentUserInfo().HasRole(role);
    }

    public bool HasClaim(string claimType)
    {
        return GetCurrentUserInfo().HasClaim(claimType);
    }

    public string? GetClaimValue(string claimType)
    {
        return GetCurrentUserInfo().GetClaimValue(claimType);
    }
} 