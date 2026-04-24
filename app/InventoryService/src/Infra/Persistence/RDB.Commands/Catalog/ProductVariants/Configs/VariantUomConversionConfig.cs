namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VariantUomConversionConfig : IEntityTypeConfiguration<VariantUomConversion>
{
    public void Configure(EntityTypeBuilder<VariantUomConversion> builder)
    {
        builder.ToTable("VariantUomConversions");
        builder.HasIndex(x => new { x.VariantRef, x.FromUomRef, x.ToUomRef }).IsUnique();
    }
}
