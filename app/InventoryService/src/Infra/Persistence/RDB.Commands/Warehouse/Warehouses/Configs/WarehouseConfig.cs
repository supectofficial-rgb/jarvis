namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.Warehouses.Configs;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class WarehouseConfig : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.ToTable("Warehouses");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
