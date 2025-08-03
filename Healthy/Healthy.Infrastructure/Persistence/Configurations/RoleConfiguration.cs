using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Healthy.Domain.Entities;

namespace Healthy.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        // Primary key is inherited from EntityBase

        builder.Property(r => r.Name)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(r => r.Description)
            .HasMaxLength(255);

        builder.Property(r => r.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(r => r.Name)
            .IsUnique()
            .HasDatabaseName("IX_Roles_Name");

        // Relationships
        builder.HasMany(r => r.UserRoles)
            .WithOne(ur => ur.Role)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query filter
        builder.HasQueryFilter(r => r.IsActive);

        // Seed data - temporarily commented out
        /*
        builder.HasData(
            new Role
            {
                Id = Guid.Parse("1a2b3c4d-5e6f-7890-abcd-ef1234567890"),
                Name = "Admin",
                Description = "System administrator with full access",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new Role
            {
                Id = Guid.Parse("2b3c4d5e-6f7a-8901-bcde-f23456789012"),
                Name = "User",
                Description = "Standard user with limited access",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
        */
    }
} 