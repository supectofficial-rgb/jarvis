namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Tags.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Tags.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class TagReadModelConfig : IEntityTypeConfiguration<TagReadModel>
{
    public void Configure(EntityTypeBuilder<TagReadModel> builder)
    {
        builder.ToTable("Tags");
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.TagName).IsUnique();
        builder.Property(x => x.BusinessKey).IsRequired();
        builder.Property(x => x.TagName).HasMaxLength(256);
        builder.Property(x => x.TagColor).HasMaxLength(64);
    }
}
