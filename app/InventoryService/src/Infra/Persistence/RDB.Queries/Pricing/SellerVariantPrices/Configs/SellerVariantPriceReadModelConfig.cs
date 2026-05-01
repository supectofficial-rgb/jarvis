namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.SellerVariantPrices.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.SellerVariantPrices.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SellerVariantPriceReadModelConfig : IEntityTypeConfiguration<SellerVariantPriceReadModel>
{
    public void Configure(EntityTypeBuilder<SellerVariantPriceReadModel> builder)
    {
        builder.ToTable("SellerVariantPrices");
        builder.HasIndex(x => new { x.SellerRef, x.VariantRef, x.PriceTypeRef, x.PriceChannelRef }).IsUnique();
        builder.HasIndex(x => x.VariantRef);
        builder.HasIndex(x => x.SellerRef);
    }
}
