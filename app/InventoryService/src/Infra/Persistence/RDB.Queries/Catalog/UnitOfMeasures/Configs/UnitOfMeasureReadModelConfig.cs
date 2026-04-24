namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.UnitOfMeasures.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.UnitOfMeasures.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class UnitOfMeasureReadModelConfig : IEntityTypeConfiguration<UnitOfMeasureReadModel>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasureReadModel> builder)
    {
        builder.ToTable("UnitOfMeasures");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
