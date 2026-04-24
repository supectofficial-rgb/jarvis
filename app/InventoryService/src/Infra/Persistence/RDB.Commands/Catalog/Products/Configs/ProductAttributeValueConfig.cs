namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Products.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProductAttributeValueConfig : IEntityTypeConfiguration<ProductAttributeValue>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValue> builder)
    {
        builder.ToTable("ProductAttributeValues");
        builder.HasIndex(x => new { x.ProductRef, x.AttributeRef }).IsUnique();
    }
}
