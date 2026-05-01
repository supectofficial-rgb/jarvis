namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceTypes.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceTypes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class PriceTypeReadModelConfig : IEntityTypeConfiguration<PriceTypeReadModel>
{
    public void Configure(EntityTypeBuilder<PriceTypeReadModel> builder)
    {
        builder.ToTable("PriceTypes");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
