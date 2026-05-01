namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Pricing.SellerVariantPrices.Configs;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class OfferConfig : IEntityTypeConfiguration<Offer>
{
    public void Configure(EntityTypeBuilder<Offer> builder)
    {
        builder.ToTable("Offers");
        builder.HasIndex(x => x.PriceRef);
    }
}
