namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProductAttributeValueReadModelConfig : IEntityTypeConfiguration<ProductAttributeValueReadModel>
{
    public void Configure(EntityTypeBuilder<ProductAttributeValueReadModel> builder)
    {
        builder.ToTable("ProductAttributeValues");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => new { x.ProductRef, x.AttributeRef }).IsUnique();
    }
}
