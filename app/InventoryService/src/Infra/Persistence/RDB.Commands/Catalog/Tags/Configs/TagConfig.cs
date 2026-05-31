namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Tags.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OysterFx.AppCore.Domain.ValueObjects;

public sealed class TagConfig : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.TagName).IsUnique();
        builder.Property(x => x.BusinessKey)
            .HasConversion(
                key => key.Value,
                value => BusinessKey.FromGuid(value))
            .IsRequired();
        builder.Property(x => x.TagName).HasMaxLength(256);
        builder.Property(x => x.TagColor).HasMaxLength(64);
    }
}
