namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.SerialItems.Configs;

using Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.SerialItems.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SerialItemReadModelConfig : IEntityTypeConfiguration<SerialItemReadModel>
{
    public void Configure(EntityTypeBuilder<SerialItemReadModel> builder)
    {
        builder.ToTable("SerialItems");
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => new { x.SerialNo, x.VariantRef }).IsUnique();
    }
}
