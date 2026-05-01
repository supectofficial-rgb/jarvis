namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.SellerVariantPrices.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.SellerVariantPrices.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class OfferReadModelConfig : IEntityTypeConfiguration<OfferReadModel>
{
    public void Configure(EntityTypeBuilder<OfferReadModel> builder)
    {
        builder.ToTable("Offers");
        builder.HasIndex(x => x.PriceRef);
    }
}
