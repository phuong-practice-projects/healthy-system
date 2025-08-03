using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Healthy.Application.Common.Models;

namespace Healthy.Infrastructure.Middleware;

public class UserAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserAuthorizationMiddleware> _logger;
    private readonly JwtSettings _jwtSettings;

    public UserAuthorizationMiddleware(
        RequestDelegate next,
        ILogger<UserAuthorizationMiddleware> logger,
        IOptions<JwtSettings> jwtSettings)
    {
        _next = next;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("🚀 UserAuthorizationMiddleware: Processing {Method} request to {Path}", 
            context.Request.Method, context.Request.Path);
        
        try
        {
            // Extract token from Authorization header
            var token = context.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation("🔑 UserAuthorizationMiddleware: Found JWT token, validating...");
                
                var claimsPrincipal = ValidateAndExtractClaims(token);
                if (claimsPrincipal != null)
                {
                    // Set user identity for the request
                    context.User = claimsPrincipal;
                    
                    // Add user information to HttpContext.Items for easy access
                    PopulateUserContext(context, claimsPrincipal);
                    
                    _logger.LogInformation("✅ User authenticated successfully: {UserId}", 
                        claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                }
                else
                {
                    _logger.LogWarning("❌ Invalid or expired JWT token");
                }
            }
            else
            {
                _logger.LogDebug("ℹ️ No authorization token found in request to {Path}", context.Request.Path);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "💥 Error processing authorization token for {Path}: {Message}", 
                context.Request.Path, ex.Message);
        }

        await _next(context);
    }


    private ClaimsPrincipal? ValidateAndExtractClaims(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                RequireExpirationTime = true
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            
            // Ensure it's a JWT token with correct algorithm
            if (validatedToken is JwtSecurityToken jwtToken && 
                jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return principal;
            }

            return null;
        }
        catch (SecurityTokenExpiredException)
        {
            _logger.LogWarning("JWT token has expired");
            return null;
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            _logger.LogWarning("JWT token has invalid signature");
            return null;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning("JWT token validation failed: {Error}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during JWT token validation");
            return null;
        }
    }

    private void PopulateUserContext(HttpContext context, ClaimsPrincipal principal)
    {
        // Extract common user information
        var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var userName = principal.FindFirst(ClaimTypes.Name)?.Value;
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        var fullName = principal.FindFirst("FullName")?.Value ?? 
                      principal.FindFirst(ClaimTypes.GivenName)?.Value ??
                      principal.FindFirst("name")?.Value ?? userName;
        var roles = principal.FindAll(ClaimTypes.Role).Select(c => c.Value).ToArray();

        // Store in HttpContext.Items for easy access throughout the request pipeline
        context.Items["UserId"] = userId;
        context.Items["UserName"] = userName;
        context.Items["UserEmail"] = email;
        context.Items["UserFullName"] = fullName;
        context.Items["UserRoles"] = roles;
        context.Items["IsAuthenticated"] = true;

        // Create CurrentUserInfo and populate CurrentUserService
        var currentUserInfo = new CurrentUserInfo
        {
            UserId = userId,
            UserName = userName,
            Email = email,
            FullName = fullName,
            IsAuthenticated = true,
            Roles = [.. roles],
            Claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value)
        };

        // Try to get token expiration
        var expClaim = principal.FindFirst("exp");
        if (expClaim != null && long.TryParse(expClaim.Value, out var exp))
        {
            currentUserInfo.TokenExpiration = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
        }

        // Store CurrentUserInfo in HttpContext.Items for CurrentUserService to access
        context.Items["CurrentUserInfo"] = currentUserInfo;

        // Log user activity
        _logger.LogInformation("User context populated - UserId: {UserId}, UserName: {UserName}, Roles: {Roles}, ItemsCount: {ItemsCount}", 
            userId, userName, string.Join(", ", roles), context.Items.Count);
    }
}
