namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.QualityStatuses.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.QualityStatuses.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class QualityStatusReadModelConfig : IEntityTypeConfiguration<QualityStatusReadModel>
{
    public void Configure(EntityTypeBuilder<QualityStatusReadModel> builder)
    {
        builder.ToTable("QualityStatuses");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
