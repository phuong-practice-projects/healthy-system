using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Healthy.Application.Common.Models;

namespace Healthy.Api.Extensions;

public static class AuthenticationExtensions
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services)
    {
        // Configure JWT Settings
        services.Configure<JwtSettings>(options =>
        {
            var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                            Environment.GetEnvironmentVariable("JwtSettings__SecretKey") ??
                            "dev-super-secret-key-with-at-least-32-characters-for-development";

            // Ensure the key has sufficient length for JWT
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 16)
            {
                secretKey = "dev-super-secret-key-with-at-least-32-characters-for-development";
            }

            options.SecretKey = secretKey;
            options.Issuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                             Environment.GetEnvironmentVariable("JwtSettings__Issuer") ??
                             "HealthySystem";
            options.Audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
                              Environment.GetEnvironmentVariable("JwtSettings__Audience") ??
                              "HealthySystemUsers";
            options.ExpirationInMinutes = int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRY_HOURS"), out var hours) ? hours * 60 : 60;
            options.RefreshTokenExpirationInDays = 7;
        });

        // Add JWT Authentication
        var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                        Environment.GetEnvironmentVariable("JwtSettings__SecretKey") ??
                        "dev-super-secret-key-with-at-least-32-characters-for-development";

        // Ensure the key has sufficient length for JWT
        if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 16)
        {
            secretKey = "dev-super-secret-key-with-at-least-32-characters-for-development";
        }

        var key = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            options.RequireHttpsMetadata = environment != "Development"; // Only require HTTPS in production
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ??
                             Environment.GetEnvironmentVariable("JwtSettings__Issuer") ??
                             "HealthySystem",
                ValidateAudience = true,
                ValidAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ??
                              Environment.GetEnvironmentVariable("JwtSettings__Audience") ??
                              "HealthySystemUsers",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Add Authorization
        services.AddAuthorization();

        return services;
    }
}
