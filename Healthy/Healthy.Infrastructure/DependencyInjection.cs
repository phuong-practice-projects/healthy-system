using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Healthy.Application.Common.Interfaces;
using Healthy.Infrastructure.Persistence;
using Healthy.Infrastructure.Services;

namespace Healthy.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Get connection string from environment variable or configuration (no fallback defaults)
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
            ?? configuration.GetConnectionString("DefaultConnection");
        
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured. Please set ConnectionStrings__DefaultConnection environment variable or add it to appsettings.json");
        }
        
        Console.WriteLine($"[DB] Using connection string: {MaskConnectionString(connectionString)}");
        
        // Add Database Context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly("Healthy.Infrastructure")
                      .EnableRetryOnFailure(
                          maxRetryCount: 5,
                          maxRetryDelay: TimeSpan.FromSeconds(30),
                          errorNumbersToAdd: null)));

        // Add Application DbContext Interface
        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        // Add Services
        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<DatabaseSeeder>();

        return services;
    }

    /// <summary>
    /// Mask sensitive information in connection string for logging
    /// </summary>
    private static string MaskConnectionString(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
            return "Empty connection string";

        // Use regex to mask any password in connection string
        return System.Text.RegularExpressions.Regex.Replace(
            connectionString, 
            @"Password=([^;]+)", 
            "Password=***", 
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);
    }
} 