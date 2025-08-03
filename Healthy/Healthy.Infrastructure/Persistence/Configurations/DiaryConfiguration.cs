using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Healthy.Domain.Entities;

namespace Healthy.Infrastructure.Persistence.Configurations;

public class DiaryConfiguration : IEntityTypeConfiguration<Diary>
{
    public void Configure(EntityTypeBuilder<Diary> builder)
    {
        builder.ToTable("Diaries");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.UserId)
            .IsRequired();

        builder.Property(d => d.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(d => d.Content)
            .IsRequired();

        builder.Property(d => d.Tags)
            .HasMaxLength(500);

        builder.Property(d => d.Mood)
            .HasMaxLength(100);

        builder.Property(d => d.IsPrivate)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(d => d.DiaryDate)
            .IsRequired();

        // Indexes
        builder.HasIndex(d => d.UserId);
        builder.HasIndex(d => d.DiaryDate);
        builder.HasIndex(d => d.IsPrivate);
        builder.HasIndex(d => new { d.UserId, d.DiaryDate });

        // Relationships
        builder.HasOne(d => d.User)
            .WithMany(u => u.Diaries)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Query filter
        builder.HasQueryFilter(d => !d.IsDeleted);
    }
}
