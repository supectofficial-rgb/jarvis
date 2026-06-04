namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.StockDetails;

using Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
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
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(stockDetailBusinessKey));
    }

    public Task<bool> ExistsByVariantRefAsync(Guid variantRef, bool onlyActive = true)
    {
        var query = _dbContext.StockDetails.Where(x => x.VariantRef == variantRef);
        if (onlyActive)
            query = query.Where(x => x.QuantityOnHand > 0);

        return query.AnyAsync();
    }

    public Task<StockDetail?> FindByBucketAsync(
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo)
    {
        IQueryable<StockDetail> query = _dbContext.StockDetails
            .Where(x =>
                x.VariantRef == variantRef &&
                x.WarehouseRef == warehouseRef &&
                x.LocationRef == locationRef &&
                x.QualityStatusRef == qualityStatusRef);

        var normalizedLotBatchNo = string.IsNullOrWhiteSpace(lotBatchNo) ? null : lotBatchNo.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedLotBatchNo))
        {
            query = query.Where(x => x.LotBatchNo == normalizedLotBatchNo);
        }

        return query
            .OrderBy(x => x.FirstReceivedAt)
            .ThenBy(x => x.LastUpdatedAt)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync();
    }

    public Task<StockDetail?> FindExactByBucketAsync(
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo)
    {
        IQueryable<StockDetail> query = _dbContext.StockDetails
            .Where(x =>
                x.VariantRef == variantRef &&
                x.SellerRef == sellerRef &&
                x.WarehouseRef == warehouseRef &&
                x.LocationRef == locationRef &&
                x.QualityStatusRef == qualityStatusRef);

        var normalizedLotBatchNo = string.IsNullOrWhiteSpace(lotBatchNo) ? null : lotBatchNo.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedLotBatchNo))
        {
            query = query.Where(x => x.LotBatchNo == normalizedLotBatchNo);
        }
        else
        {
            query = query.Where(x => x.LotBatchNo == null);
        }

        return query
            .OrderBy(x => x.FirstReceivedAt)
            .ThenBy(x => x.LastUpdatedAt)
            .ThenBy(x => x.Id)
            .FirstOrDefaultAsync();
    }
}
