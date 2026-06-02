namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.ProductVariants.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ProductVariantConfig : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");
        builder.HasIndex(x => x.VariantSku).IsUnique();
        builder.HasIndex(x => x.Barcode).IsUnique();

        builder.HasMany(x => x.AttributeValues)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.UomConversions)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Components)
            .WithOne()
            .HasForeignKey(x => x.VariantRef)
            .HasPrincipalKey(x => x.BusinessKey)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.AddOns)
            .WithOne()
            .HasForeignKey(x => x.VariantRef)
            .HasPrincipalKey(x => x.BusinessKey)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Images)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Tags)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
