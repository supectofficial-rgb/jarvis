namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceChannels.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceChannels.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PriceChannelReadModelConfig : IEntityTypeConfiguration<PriceChannelReadModel>
{
    public void Configure(EntityTypeBuilder<PriceChannelReadModel> builder)
    {
        builder.ToTable("PriceChannels");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
