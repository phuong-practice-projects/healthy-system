using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Healthy.Domain.Entities;

namespace Healthy.Infrastructure.Persistence.Configurations;

public class BodyRecordConfiguration : IEntityTypeConfiguration<BodyRecord>
{
    public void Configure(EntityTypeBuilder<BodyRecord> builder)
    {
        builder.ToTable("BodyRecords");

        builder.HasKey(br => br.Id);

        builder.Property(br => br.UserId)
            .IsRequired();

        builder.Property(br => br.Weight)
            .IsRequired()
            .HasPrecision(5, 2);

        builder.Property(br => br.BodyFatPercentage)
            .HasPrecision(5, 2);

        builder.Property(br => br.RecordDate)
            .IsRequired();

        builder.Property(br => br.Notes)
            .HasMaxLength(500);

        // Indexes
        builder.HasIndex(br => br.UserId);
        builder.HasIndex(br => br.RecordDate);
        builder.HasIndex(br => new { br.UserId, br.RecordDate });

        // Relationships
        builder.HasOne(br => br.User)
            .WithMany(u => u.BodyRecords)
            .HasForeignKey(br => br.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Index for DeletedAt
        builder.HasIndex(br => br.DeletedAt);

        // Query filter
        builder.HasQueryFilter(br => br.DeletedAt == null);
    }
}
