namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.Products.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProductConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasIndex(x => x.BaseSku).IsUnique();
        builder.HasIndex(x => x.CategoryRef);
        builder.HasIndex(x => x.CategorySchemaVersionRef);
        builder.Property(x => x.ImageFileKey).HasMaxLength(256);
        builder.Property(x => x.ImageUrl).HasMaxLength(1024);
        builder.Property(x => x.ImageThumbnailUrl).HasMaxLength(1024);

        builder.HasMany(x => x.AttributeValues)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
