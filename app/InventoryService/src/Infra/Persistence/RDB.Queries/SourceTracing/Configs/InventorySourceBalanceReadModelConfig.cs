namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventorySourceBalanceReadModelConfig : IEntityTypeConfiguration<InventorySourceBalanceReadModel>
{
    public void Configure(EntityTypeBuilder<InventorySourceBalanceReadModel> builder)
    {
        builder.ToTable("InventorySourceBalances");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
    }
}
