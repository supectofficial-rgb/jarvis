namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Pricing.PriceChannels.Configs;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PriceChannelConfig : IEntityTypeConfiguration<PriceChannel>
{
    public void Configure(EntityTypeBuilder<PriceChannel> builder)
    {
        builder.ToTable("PriceChannels");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
