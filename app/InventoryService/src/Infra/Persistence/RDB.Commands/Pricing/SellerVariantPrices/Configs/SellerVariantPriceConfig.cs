namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Pricing.SellerVariantPrices.Configs;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SellerVariantPriceConfig : IEntityTypeConfiguration<SellerVariantPrice>
{
    public void Configure(EntityTypeBuilder<SellerVariantPrice> builder)
    {
        builder.ToTable("SellerVariantPrices");
        builder.HasIndex(x => new { x.SellerRef, x.VariantRef, x.PriceTypeRef, x.PriceChannelRef }).IsUnique();
        builder.HasIndex(x => x.VariantRef);
        builder.HasIndex(x => x.SellerRef);

        builder.HasMany(x => x.Offers)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(x => x.Offers).UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
