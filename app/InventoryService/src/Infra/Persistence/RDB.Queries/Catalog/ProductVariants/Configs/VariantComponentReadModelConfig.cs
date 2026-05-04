namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VariantComponentReadModelConfig : IEntityTypeConfiguration<VariantComponentReadModel>
{
    public void Configure(EntityTypeBuilder<VariantComponentReadModel> builder)
    {
        builder.ToTable("VariantComponents");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => new { x.VariantRef, x.ComponentVariantRef }).IsUnique();
    }
}
