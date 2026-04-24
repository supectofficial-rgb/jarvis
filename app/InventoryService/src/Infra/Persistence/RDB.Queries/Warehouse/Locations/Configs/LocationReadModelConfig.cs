namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Locations.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Locations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LocationReadModelConfig : IEntityTypeConfiguration<LocationReadModel>
{
    public void Configure(EntityTypeBuilder<LocationReadModel> builder)
    {
        builder.ToTable("Locations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.LocationCode).IsUnique();
    }
}
