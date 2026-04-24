namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Reservations.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Reservations.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class ReservationAllocationReadModelConfig : IEntityTypeConfiguration<ReservationAllocationReadModel>
{
    public void Configure(EntityTypeBuilder<ReservationAllocationReadModel> builder)
    {
        builder.ToTable("ReservationAllocations");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
    }
}
