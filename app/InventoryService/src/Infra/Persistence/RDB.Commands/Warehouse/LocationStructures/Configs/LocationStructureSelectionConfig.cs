namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.LocationStructures.Configs;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LocationStructureSelectionConfig : IEntityTypeConfiguration<LocationStructureSelection>
{
    public void Configure(EntityTypeBuilder<LocationStructureSelection> builder)
    {
        builder.ToTable("LocationStructureSelections");
        builder.HasIndex(x => new { x.LocationRef, x.StructureRef }).IsUnique();
    }
}
