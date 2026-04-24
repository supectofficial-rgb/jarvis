namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StockDetailReadModelConfig : IEntityTypeConfiguration<StockDetailReadModel>
{
    public void Configure(EntityTypeBuilder<StockDetailReadModel> builder)
    {
        builder.ToTable("StockDetails");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
    }
}

