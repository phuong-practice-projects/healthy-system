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
        // Get connection string from environment variables or configuration
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
            ?? configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost,1433;Database=HealthyDB_Dev;User Id=sa;Password=Dev@Passw0rd123;TrustServerCertificate=true;";
        
        // Add Database Context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly("Healthy.Infrastructure")
                      .EnableRetryOnFailure(
                          maxRetryCount: 3,
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
} 