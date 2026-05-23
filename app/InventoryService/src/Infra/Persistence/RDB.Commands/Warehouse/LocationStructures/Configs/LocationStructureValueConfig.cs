namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.LocationStructures.Configs;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LocationStructureValueConfig : IEntityTypeConfiguration<LocationStructureValue>
{
    public void Configure(EntityTypeBuilder<LocationStructureValue> builder)
    {
        builder.ToTable("LocationStructureValues");
        builder.HasIndex(x => new { x.StructureRef, x.Code }).IsUnique();
        builder.HasIndex(x => new { x.StructureRef, x.DisplayOrder });
    }
}
