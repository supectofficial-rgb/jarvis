namespace OysterFx.Infra.Persistence.RDB.Commands.Configs;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OysterFx.Infra.Persistence.EventSourcing.Abstractions;

public class OutboxEventConfig : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.HasKey(c => c.OutBoxEventItemId);
        builder.Property(c => c.AccuredByUserId).HasMaxLength(255);
        builder.Property(c => c.EventName).HasMaxLength(255);
        builder.Property(c => c.AggregateName).HasMaxLength(255);
        builder.Property(c => c.EventTypeName).HasMaxLength(500);
        builder.Property(c => c.AggregateTypeName).HasMaxLength(500);
        builder.Property(c => c.TraceId).HasMaxLength(100);
        builder.Property(c => c.SpanId).HasMaxLength(100);
        builder.ToTable("OutboxEvents", "SkyPearlFx");
    }
}