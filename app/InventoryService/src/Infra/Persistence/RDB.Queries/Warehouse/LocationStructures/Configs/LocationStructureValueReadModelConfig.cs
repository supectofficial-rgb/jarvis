namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LocationStructureValueReadModelConfig : IEntityTypeConfiguration<LocationStructureValueReadModel>
{
    public void Configure(EntityTypeBuilder<LocationStructureValueReadModel> builder)
    {
        builder.ToTable("LocationStructureValues");
        builder.HasIndex(x => new { x.StructureRef, x.Code }).IsUnique();
    }
}
