namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LocationStructureNodeReadModelConfig : IEntityTypeConfiguration<LocationStructureNodeReadModel>
{
    public void Configure(EntityTypeBuilder<LocationStructureNodeReadModel> builder)
    {
        builder.ToTable("LocationStructureNodes");
        builder.HasIndex(x => new { x.WarehouseRef, x.Code }).IsUnique();
    }
}
