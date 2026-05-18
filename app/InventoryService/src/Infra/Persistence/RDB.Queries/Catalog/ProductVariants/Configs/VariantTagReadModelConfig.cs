namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VariantTagReadModelConfig : IEntityTypeConfiguration<VariantTagReadModel>
{
    public void Configure(EntityTypeBuilder<VariantTagReadModel> builder)
    {
        builder.ToTable("VariantTags");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => new { x.VariantRef, x.TagName }).IsUnique();
        builder.Property(x => x.TagName).HasMaxLength(256);
        builder.Property(x => x.TagColor).HasMaxLength(64);
    }
}
