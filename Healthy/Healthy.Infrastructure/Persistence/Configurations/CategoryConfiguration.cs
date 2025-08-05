using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Healthy.Domain.Entities;

namespace Healthy.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories");

        // Primary key is inherited from EntityAuditableBase

        // Configure ImageUrl
        builder.Property(c => c.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        // Configure Description
        builder.Property(c => c.Description)
            .IsRequired()
            .HasMaxLength(1000);

        // Configure CategoryType
        builder.Property(c => c.CategoryType)
            .IsRequired(false)
            .HasMaxLength(100);

        // Configure PublishedAt
        builder.Property(c => c.PublishedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        // Configure IsActive
        builder.Property(c => c.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Configure Tags
        builder.Property(c => c.Tags)
            .IsRequired()
            .HasMaxLength(500);

        // Configure audit fields from EntityAuditableBase
        builder.Property(c => c.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(c => c.UpdatedAt)
            .IsRequired(false)
            .HasColumnType("datetime2");

        builder.Property(c => c.CreatedBy)
            .IsRequired(false)
            .HasMaxLength(450);

        builder.Property(c => c.UpdatedBy)
            .IsRequired(false)
            .HasMaxLength(450);

        builder.Property(c => c.DeletedAt)
            .IsRequired(false)
            .HasColumnType("datetime2");

        builder.Property(c => c.DeletedBy)
            .IsRequired(false)
            .HasMaxLength(450);

        // Configure indexes
        builder.HasIndex(c => c.DeletedAt)
            .HasDatabaseName("IX_Categories_DeletedAt");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("IX_Categories_IsActive");

        builder.HasIndex(c => c.CategoryType)
            .HasDatabaseName("IX_Categories_CategoryType");

        builder.HasIndex(c => c.PublishedAt)
            .HasDatabaseName("IX_Categories_PublishedAt");

        builder.HasIndex(c => c.CreatedAt)
            .HasDatabaseName("IX_Categories_CreatedAt");

        // Global query filter for soft delete
        builder.HasQueryFilter(c => c.DeletedAt == null);
    }
}
