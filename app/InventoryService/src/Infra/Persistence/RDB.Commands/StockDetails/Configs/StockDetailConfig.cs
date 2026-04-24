namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.StockDetails.Configs;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public sealed class StockDetailConfig : IEntityTypeConfiguration<StockDetail>
{
    public void Configure(EntityTypeBuilder<StockDetail> builder)
    {
        builder.ToTable("StockDetails");
        builder.HasIndex(x => new { x.VariantRef, x.SellerRef, x.WarehouseRef, x.LocationRef, x.QualityStatusRef, x.LotBatchNo })
            .IsUnique();
    }
}
