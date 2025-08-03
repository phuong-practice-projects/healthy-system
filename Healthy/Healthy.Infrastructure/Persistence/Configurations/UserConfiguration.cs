using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Healthy.Domain.Entities;

namespace Healthy.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PasswordHash)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        builder.Property(u => u.Gender)
            .HasMaxLength(10);

        builder.Property(u => u.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure audit fields from EntityAuditableBase
        builder.Property(u => u.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(u => u.UpdatedAt)
            .IsRequired(false)
            .HasColumnType("datetime2");

        builder.Property(u => u.CreatedBy)
            .IsRequired(false)
            .HasMaxLength(450);

        builder.Property(u => u.UpdatedBy)
            .IsRequired(false)
            .HasMaxLength(450);

        builder.Property(u => u.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(u => u.DeletedAt)
            .IsRequired(false)
            .HasColumnType("datetime2");

        builder.Property(u => u.DeletedBy)
            .IsRequired(false)
            .HasMaxLength(450);

        // Indexes
        builder.HasIndex(u => u.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email");

        builder.HasIndex(u => u.PhoneNumber)
            .HasDatabaseName("IX_Users_PhoneNumber");

        builder.HasIndex(u => u.IsDeleted)
            .HasDatabaseName("IX_Users_IsDeleted");

        builder.HasIndex(u => u.CreatedAt)
            .HasDatabaseName("IX_Users_CreatedAt");

        // Relationships
        builder.HasMany(u => u.UserRoles)
            .WithOne(ur => ur.User)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query filter for soft delete
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
} 