namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VariantComponentConfig : IEntityTypeConfiguration<VariantComponent>
{
    public void Configure(EntityTypeBuilder<VariantComponent> builder)
    {
        builder.ToTable("VariantComponents");
        builder.HasIndex(x => new { x.VariantRef, x.ComponentVariantRef }).IsUnique();
    }
}
