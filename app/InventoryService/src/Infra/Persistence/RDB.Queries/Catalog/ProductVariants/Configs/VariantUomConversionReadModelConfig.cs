namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VariantUomConversionReadModelConfig : IEntityTypeConfiguration<VariantUomConversionReadModel>
{
    public void Configure(EntityTypeBuilder<VariantUomConversionReadModel> builder)
    {
        builder.ToTable("VariantUomConversions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => new { x.VariantRef, x.FromUomRef, x.ToUomRef }).IsUnique();
    }
}
