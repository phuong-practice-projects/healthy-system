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
        // Get connection string from environment variable
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") 
            ?? "Server=localhost;Database=HealthyDB;User Id=sa;Password=;TrustServerCertificate=true;";
        
        // Add Database Context
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                connectionString,
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
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

        return services;
    }
} 