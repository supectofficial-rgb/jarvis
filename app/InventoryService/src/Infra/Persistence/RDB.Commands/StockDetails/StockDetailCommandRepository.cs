namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.StockDetails;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Commands;

public class StockDetailCommandRepository
    : CommandRepository<StockDetail, InventoryServiceCommandDbContext>, IStockDetailCommandRepository
{
    public StockDetailCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<StockDetail?> GetByBusinessKeyAsync(Guid stockDetailBusinessKey)
    {
        return _dbContext.StockDetails
            .FirstOrDefaultAsync(x => x.BusinessKey.Value == stockDetailBusinessKey);
    }

    public Task<StockDetail?> FindByBucketAsync(
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo)
    {
        return _dbContext.StockDetails.FirstOrDefaultAsync(x =>
            x.VariantRef == variantRef &&
            x.SellerRef == sellerRef &&
            x.WarehouseRef == warehouseRef &&
            x.LocationRef == locationRef &&
            x.QualityStatusRef == qualityStatusRef &&
            x.LotBatchNo == lotBatchNo);
    }
}
