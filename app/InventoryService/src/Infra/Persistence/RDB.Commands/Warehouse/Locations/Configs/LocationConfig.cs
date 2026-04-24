namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.Locations.Configs;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class LocationConfig : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("Locations");
        builder.HasIndex(x => x.LocationCode).IsUnique();
    }
}
