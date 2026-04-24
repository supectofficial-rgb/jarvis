namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventorySourceConsumptionReadModelConfig : IEntityTypeConfiguration<InventorySourceConsumptionReadModel>
{
    public void Configure(EntityTypeBuilder<InventorySourceConsumptionReadModel> builder)
    {
        builder.ToTable("InventorySourceConsumptions");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
    }
}
