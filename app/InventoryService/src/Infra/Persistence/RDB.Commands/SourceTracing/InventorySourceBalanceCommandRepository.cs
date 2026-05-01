namespace Insurance.InventoryService.Infra.Persistence.RDB.Commands.SourceTracing;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Commands;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Infra.Persistence.RDB.Commands;

public class InventorySourceBalanceCommandRepository
    : CommandRepository<InventorySourceBalance, InventoryServiceCommandDbContext>, IInventorySourceBalanceCommandRepository
{
    public InventorySourceBalanceCommandRepository(InventoryServiceCommandDbContext dbContext)
        : base(dbContext)
    {
    }

    public Task<InventorySourceBalance?> GetByBusinessKeyAsync(Guid sourceBalanceBusinessKey)
    {
        return _dbContext.InventorySourceBalances
            .Include(x => x.Allocations)
            .Include(x => x.Consumptions)
            .FirstOrDefaultAsync(x => x.BusinessKey == BusinessKey.FromGuid(sourceBalanceBusinessKey));
    }

    public Task<List<InventorySourceBalance>> GetOpenByBucketAsync(
        Guid variantRef,
        Guid sellerRef,
        Guid warehouseRef,
        Guid locationRef,
        Guid qualityStatusRef,
        string? lotBatchNo,
        Guid? serialRef)
    {
        var normalizedLotBatchNo = string.IsNullOrWhiteSpace(lotBatchNo) ? null : lotBatchNo.Trim();

        return _dbContext.InventorySourceBalances
            .Include(x => x.Allocations)
            .Include(x => x.Consumptions)
            .Where(x =>
                x.Status == InventorySourceBalanceStatus.Open &&
                x.VariantRef == variantRef &&
                x.SellerRef == sellerRef &&
                x.WarehouseRef == warehouseRef &&
                x.LocationRef == locationRef &&
                x.QualityStatusRef == qualityStatusRef &&
                x.LotBatchNo == normalizedLotBatchNo &&
                x.SerialRef == serialRef)
            .OrderBy(x => x.OpenedAt)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }
}
