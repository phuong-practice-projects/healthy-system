using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Healthy.Domain.Entities;

namespace Healthy.Infrastructure.Persistence.Configurations;

public class ExerciseConfiguration : IEntityTypeConfiguration<Exercise>
{
    public void Configure(EntityTypeBuilder<Exercise> builder)
    {
        builder.ToTable("Exercises");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId)
            .IsRequired();

        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(e => e.Description)
            .HasMaxLength(1000);

        builder.Property(e => e.DurationMinutes)
            .IsRequired();

        builder.Property(e => e.CaloriesBurned)
            .IsRequired();

        builder.Property(e => e.ExerciseDate)
            .IsRequired();

        builder.Property(e => e.Category)
            .HasMaxLength(100);

        builder.Property(e => e.Notes)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.ExerciseDate);
        builder.HasIndex(e => e.Category);
        builder.HasIndex(e => new { e.UserId, e.ExerciseDate });

        // Relationships
        builder.HasOne(e => e.User)
            .WithMany(u => u.Exercises)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query filter
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
