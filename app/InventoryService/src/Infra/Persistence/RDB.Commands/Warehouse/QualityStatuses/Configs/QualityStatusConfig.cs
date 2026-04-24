namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Warehouse.QualityStatuses.Configs;

using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class QualityStatusConfig : IEntityTypeConfiguration<QualityStatus>
{
    public void Configure(EntityTypeBuilder<QualityStatus> builder)
    {
        builder.ToTable("QualityStatuses");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
