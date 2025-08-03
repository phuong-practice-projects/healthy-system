using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Healthy.Domain.Entities;
using Healthy.Infrastructure.Persistence;
using BCrypt.Net;

namespace Healthy.Infrastructure.Services;

public class DatabaseSeeder
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(ApplicationDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();
            
            // Check if data already exists
            if (await _context.Roles.AnyAsync())
            {
                _logger.LogInformation("Database already seeded, skipping seed process");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // Seed Roles
            await SeedRolesAsync();
            
            // Seed Users
            await SeedUsersAsync();
            
            // Seed UserRoles
            await SeedUserRolesAsync();

            await _context.SaveChangesAsync();
            
            _logger.LogInformation("Database seeding completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedRolesAsync()
    {
        var roles = new[]
        {
            new Role
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Admin",
                Description = "System Administrator with full access",
                IsActive = true,
            },
            new Role
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "User",
                Description = "Regular user with basic access",
                IsActive = true,
            },
            new Role
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Moderator",
                Description = "Moderator with limited admin access",
                IsActive = true
            }
        };

        await _context.Roles.AddRangeAsync(roles);
        _logger.LogInformation("Seeded {Count} roles", roles.Length);
    }

    private async Task SeedUsersAsync()
    {
        var users = new[]
        {
            new User
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                FirstName = "System",
                LastName = "Administrator",
                Email = "admin@healthysystem.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123", 12),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new User
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                FirstName = "Test",
                LastName = "User",
                Email = "user@healthysystem.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123", 12),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            new User
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                FirstName = "Test",
                LastName = "Moderator",
                Email = "moderator@healthysystem.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Moderator@123", 12),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        };

        await _context.Users.AddRangeAsync(users);
        _logger.LogInformation("Seeded {Count} users", users.Length);
    }

    private async Task SeedUserRolesAsync()
    {
        var userRoles = new[]
        {
            // Admin user -> Admin role
            new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                RoleId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            // Test user -> User role
            new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            },
            // Moderator user -> Moderator role
            new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                RoleId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            }
        };

        await _context.UserRoles.AddRangeAsync(userRoles);
        _logger.LogInformation("Seeded {Count} user roles", userRoles.Length);
    }

    public async Task<bool> IsDatabaseSeededAsync()
    {
        return await _context.Roles.AnyAsync() && await _context.Users.AnyAsync();
    }

    public async Task ClearDataAsync()
    {
        _logger.LogInformation("Clearing all seed data...");
        
        _context.UserRoles.RemoveRange(_context.UserRoles);
        _context.Users.RemoveRange(_context.Users);
        _context.Roles.RemoveRange(_context.Roles);
        
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("All seed data cleared");
    }
}
