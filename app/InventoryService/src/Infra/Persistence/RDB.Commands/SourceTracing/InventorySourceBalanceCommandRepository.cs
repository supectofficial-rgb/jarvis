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

    public Task<List<InventorySourceBalance>> GetByReservationRefAsync(Guid reservationRef)
    {
        return _dbContext.InventorySourceBalances
            .Include(x => x.Allocations)
            .Include(x => x.Consumptions)
            .Where(x => x.Allocations.Any(a => a.ReservationRef == reservationRef))
            .OrderBy(x => x.OpenedAt)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }

    public Task<List<InventorySourceBalance>> GetOpenByPoolAsync(
        Guid variantRef,
        Guid warehouseRef,
        Guid? qualityStatusRef = null,
        string? lotBatchNo = null)
    {
        IQueryable<InventorySourceBalance> query = _dbContext.InventorySourceBalances
            .Include(x => x.Allocations)
            .Include(x => x.Consumptions)
            .Where(x =>
                x.Status == InventorySourceBalanceStatus.Open &&
                x.VariantRef == variantRef &&
                x.WarehouseRef == warehouseRef);

        if (qualityStatusRef.HasValue)
            query = query.Where(x => x.QualityStatusRef == qualityStatusRef.Value);

        var normalizedLotBatchNo = string.IsNullOrWhiteSpace(lotBatchNo) ? null : lotBatchNo.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedLotBatchNo))
            query = query.Where(x => x.LotBatchNo == normalizedLotBatchNo);

        return query
            .OrderBy(x => x.OpenedAt)
            .ThenBy(x => x.Id)
            .ToListAsync();
    }
}
