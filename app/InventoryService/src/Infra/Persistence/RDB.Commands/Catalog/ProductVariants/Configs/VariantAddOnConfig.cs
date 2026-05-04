namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VariantAddOnConfig : IEntityTypeConfiguration<VariantAddOn>
{
    public void Configure(EntityTypeBuilder<VariantAddOn> builder)
    {
        builder.ToTable("VariantAddOns");
        builder.HasIndex(x => new { x.VariantRef, x.AddOnVariantRef }).IsUnique();
    }
}
