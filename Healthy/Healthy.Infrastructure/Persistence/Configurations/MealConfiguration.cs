using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Healthy.Domain.Entities;

namespace Healthy.Infrastructure.Persistence.Configurations;

public class MealConfiguration : IEntityTypeConfiguration<Meal>
{
    public void Configure(EntityTypeBuilder<Meal> builder)
    {
        // Table name
        builder.ToTable("Meals");

        // Primary key
        builder.HasKey(m => m.Id);

        // Properties
        builder.Property(m => m.Id)
            .IsRequired()
            .ValueGeneratedOnAdd();

        builder.Property(m => m.UserId)
            .IsRequired();

        builder.Property(m => m.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(m => m.Type)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(m => m.Date)
            .IsRequired()
            .HasColumnType("datetime2");

        builder.Property(m => m.CreatedAt)
            .IsRequired()
            .HasColumnType("datetime2")
            .HasDefaultValueSql("GETUTCDATE()");

        builder.Property(m => m.UpdatedAt)
            .HasColumnType("datetime2");

        // Relationships
        builder.HasOne(m => m.User)
            .WithMany(u => u.Meals)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        builder.HasIndex(m => m.UserId)
            .HasDatabaseName("IX_Meals_UserId");

        builder.HasIndex(m => new { m.UserId, m.Date })
            .HasDatabaseName("IX_Meals_UserId_Date");

        builder.HasIndex(m => new { m.UserId, m.Type })
            .HasDatabaseName("IX_Meals_UserId_Type");
    }
}
