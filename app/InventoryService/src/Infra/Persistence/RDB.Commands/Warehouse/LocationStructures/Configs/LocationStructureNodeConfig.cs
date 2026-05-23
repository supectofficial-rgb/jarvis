namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.LocationStructures.Configs;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LocationStructureNodeConfig : IEntityTypeConfiguration<LocationStructureNode>
{
    public void Configure(EntityTypeBuilder<LocationStructureNode> builder)
    {
        builder.ToTable("LocationStructureNodes");
        builder.HasIndex(x => new { x.WarehouseRef, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.WarehouseRef, x.ParentStructureRef, x.DisplayOrder });
    }
}
