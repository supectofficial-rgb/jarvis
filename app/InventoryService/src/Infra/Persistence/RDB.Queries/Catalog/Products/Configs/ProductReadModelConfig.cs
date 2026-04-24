namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.Products.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProductReadModelConfig : IEntityTypeConfiguration<ProductReadModel>
{
    public void Configure(EntityTypeBuilder<ProductReadModel> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.BaseSku).IsUnique();
        builder.HasIndex(x => x.CategoryRef);
        builder.HasIndex(x => x.CategorySchemaVersionRef);
    }
}
