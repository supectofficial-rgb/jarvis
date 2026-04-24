namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.Catalog.UnitOfMeasures.Configs;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class UnitOfMeasureConfig : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.ToTable("UnitOfMeasures");
        builder.HasIndex(x => x.Code).IsUnique();
    }
}
