namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.StockDetails.SerialItems.Configs;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class SerialItemConfig : IEntityTypeConfiguration<SerialItem>
{
    public void Configure(EntityTypeBuilder<SerialItem> builder)
    {
        builder.ToTable("SerialItems");
        builder.HasIndex(x => x.BusinessKey).IsUnique();
        builder.HasIndex(x => new { x.SerialNo, x.VariantRef }).IsUnique();
        builder.Property(x => x.SerialNo).HasMaxLength(128).IsRequired();
        builder.Property(x => x.LotBatchNo).HasMaxLength(128);
    }
}
