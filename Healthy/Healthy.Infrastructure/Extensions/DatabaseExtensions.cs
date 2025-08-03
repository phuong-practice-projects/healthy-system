using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Healthy.Infrastructure.Services;
using Healthy.Infrastructure.Persistence;

namespace Healthy.Infrastructure.Extensions;

public static class DatabaseExtensions
{
    public static async Task<IServiceProvider> SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            var seeder = services.GetRequiredService<DatabaseSeeder>();
            
            logger.LogInformation("Starting database seeding...");
            
            // Only seed if database is empty (don't clear existing data)
            if (!await seeder.IsDatabaseSeededAsync())
            {
                await seeder.SeedAsync();
                logger.LogInformation("Database seeding completed successfully");
            }
            else
            {
                logger.LogInformation("Database already has data, skipping seeding to preserve existing data");
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
        
        return serviceProvider;
    }
    
    public static async Task<IServiceProvider> MigrateDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            
            logger.LogInformation("Applying database migrations...");
            
            // Use migrations instead of EnsureCreated
            await context.Database.MigrateAsync();
            
            logger.LogInformation("Database migrations applied successfully");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "An error occurred while migrating the database");
            throw;
        }
        
        return serviceProvider;
    }
    
    public static async Task<IServiceProvider> InitializeDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            
            logger.LogInformation("Initializing database...");
            
            // Ensure database is created
            await context.Database.EnsureCreatedAsync();
            
            logger.LogInformation("Database created successfully");
            
            // Seed the database
            await serviceProvider.SeedDatabaseAsync();
            
            logger.LogInformation("Database initialization completed");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            logger.LogError(ex, "An error occurred while initializing the database");
            throw;
        }
        
        return serviceProvider;
    }
    
    /// <summary>
    /// Force reset database and seed fresh data (use only when you want to clear all data)
    /// </summary>
    public static async Task<IServiceProvider> ResetAndSeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            var seeder = services.GetRequiredService<DatabaseSeeder>();
            
            logger.LogInformation("Resetting database and seeding fresh data...");
            
            // Clear existing data and seed fresh
            await seeder.ClearDataAsync();
            
            logger.LogInformation("Database reset and seeding completed successfully");
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<DatabaseSeeder>>();
            logger.LogError(ex, "An error occurred while resetting and seeding the database");
            throw;
        }
        
        return serviceProvider;
    }
}
