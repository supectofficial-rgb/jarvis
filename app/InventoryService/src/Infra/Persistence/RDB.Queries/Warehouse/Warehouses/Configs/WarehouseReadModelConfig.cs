namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Warehouses.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Warehouses.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class WarehouseReadModelConfig : IEntityTypeConfiguration<WarehouseReadModel>
{
    public void Configure(EntityTypeBuilder<WarehouseReadModel> builder)
    {
        builder.ToTable("Warehouses");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
