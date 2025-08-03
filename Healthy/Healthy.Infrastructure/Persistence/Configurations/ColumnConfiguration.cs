using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Healthy.Domain.Entities;

namespace Healthy.Infrastructure.Persistence.Configurations;

public class ColumnConfiguration : IEntityTypeConfiguration<Column>
{
    public void Configure(EntityTypeBuilder<Column> builder)
    {
        builder.ToTable("Columns");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(c => c.ImageUrl)
            .HasMaxLength(500);

        builder.Property(c => c.Category)
            .HasMaxLength(100);

        builder.Property(c => c.Tags)
            .HasMaxLength(500);

        builder.Property(c => c.IsPublished)
            .IsRequired()
            .HasDefaultValue(true);

        // Indexes
        builder.HasIndex(c => c.Category);
        builder.HasIndex(c => c.CreatedAt);
        builder.HasIndex(c => c.IsPublished);

        // Query filter for soft delete and published records
        builder.HasQueryFilter(c => c.IsPublished && !c.IsDeleted);
    }
}
