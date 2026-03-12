using Infrastructure.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class ProcessedEventConfiguration : IEntityTypeConfiguration<ProcessedEventEntity>
{
    public void Configure(EntityTypeBuilder<ProcessedEventEntity> builder)
    {
        builder.ToTable("processed_events");

        builder.HasKey(x => new { x.UserId, x.GameId });

        builder.Property(x => x.ProcessedAt)
            .IsRequired();
    }
}
