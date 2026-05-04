namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.ProductVariants.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VariantAddOnReadModelConfig : IEntityTypeConfiguration<VariantAddOnReadModel>
{
    public void Configure(EntityTypeBuilder<VariantAddOnReadModel> builder)
    {
        builder.ToTable("VariantAddOns");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => new { x.VariantRef, x.AddOnVariantRef }).IsUnique();
    }
}
