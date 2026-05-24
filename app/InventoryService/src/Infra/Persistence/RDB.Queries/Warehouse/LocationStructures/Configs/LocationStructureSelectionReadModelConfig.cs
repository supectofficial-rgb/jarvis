namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LocationStructureSelectionReadModelConfig : IEntityTypeConfiguration<LocationStructureSelectionReadModel>
{
    public void Configure(EntityTypeBuilder<LocationStructureSelectionReadModel> builder)
    {
        builder.ToTable("LocationStructureSelections");
    }
}
