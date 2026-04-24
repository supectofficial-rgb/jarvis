namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProductVariantReadModelConfig : IEntityTypeConfiguration<ProductVariantReadModel>
{
    public void Configure(EntityTypeBuilder<ProductVariantReadModel> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.VariantSku).IsUnique();
        builder.HasIndex(x => x.Barcode).IsUnique();
    }
}
