namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VariantTagConfig : IEntityTypeConfiguration<VariantTag>
{
    public void Configure(EntityTypeBuilder<VariantTag> builder)
    {
        builder.ToTable("VariantTags");
        builder.HasIndex(x => new { x.VariantRef, x.TagRef }).IsUnique();
        builder.HasIndex(x => x.TagRef);
        builder.Property(x => x.TagRef).IsRequired();
        builder.Property(x => x.TagName).HasMaxLength(256);
        builder.Property(x => x.TagColor).HasMaxLength(64);
    }
}
