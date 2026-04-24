namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class VariantAttributeValueConfig : IEntityTypeConfiguration<VariantAttributeValue>
{
    public void Configure(EntityTypeBuilder<VariantAttributeValue> builder)
    {
        builder.ToTable("VariantAttributeValues");
        builder.HasIndex(x => new { x.VariantRef, x.AttributeRef }).IsUnique();
    }
}
