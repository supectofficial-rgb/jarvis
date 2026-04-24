namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Reservations.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Reservations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class InventoryReservationReadModelConfig : IEntityTypeConfiguration<InventoryReservationReadModel>
{
    public void Configure(EntityTypeBuilder<InventoryReservationReadModel> builder)
    {
        builder.ToTable("InventoryReservations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
    }
}
