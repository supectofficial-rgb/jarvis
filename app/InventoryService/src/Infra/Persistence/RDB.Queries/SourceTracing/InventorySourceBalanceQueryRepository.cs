namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing;

using Insurance.InventoryService.AppCore.Domain.SourceTracing.Entities;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetByBusinessKey;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.SourceTracing.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class InventorySourceBalanceQueryRepository
    : QueryRepository<InventoryServiceQueryDbContext>, IInventorySourceBalanceQueryRepository
{
    public InventorySourceBalanceQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetInventorySourceBalanceByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid sourceBalanceBusinessKey)
    {
        var item = await _dbContext.Set<InventorySourceBalanceReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == sourceBalanceBusinessKey);

        if (item is null)
            return null;

        var mapped = ToBalanceListItem(item);
        return new GetInventorySourceBalanceByBusinessKeyQueryResult
        {
            SourceBalanceBusinessKey = mapped.SourceBalanceBusinessKey,
            SourceType = mapped.SourceType,
            VariantRef = mapped.VariantRef,
            SellerRef = mapped.SellerRef,
            WarehouseRef = mapped.WarehouseRef,
            LocationRef = mapped.LocationRef,
            QualityStatusRef = mapped.QualityStatusRef,
            LotBatchNo = mapped.LotBatchNo,
            ReceivedQty = mapped.ReceivedQty,
            AllocatedQty = mapped.AllocatedQty,
            ConsumedQty = mapped.ConsumedQty,
            AvailableQty = mapped.AvailableQty,
            RemainingQty = mapped.RemainingQty,
            Status = mapped.Status
        };
    }

    public async Task<InventorySourceBalanceListItem?> GetByIdAsync(Guid sourceBalanceId)
    {
        var item = await _dbContext.Set<InventorySourceBalanceReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == sourceBalanceId);

        return item is null ? null : ToBalanceListItem(item);
    }

    public async Task<List<InventorySourceBalanceListItem>> GetOpenByVariantAsync(Guid variantRef)
    {
        var items = await _dbContext.Set<InventorySourceBalanceReadModel>()
            .AsNoTracking()
            .Where(x => x.VariantRef == variantRef && x.Status == InventorySourceBalanceStatus.Open)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToBalanceListItem).ToList();
    }

    public async Task<InventorySourceBalanceSummaryItem?> GetSummaryAsync(Guid sourceBalanceBusinessKey)
    {
        var item = await _dbContext.Set<InventorySourceBalanceReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == sourceBalanceBusinessKey);

        return item is null
            ? null
            : new InventorySourceBalanceSummaryItem
            {
                SourceBalanceBusinessKey = item.BusinessKey,
                ReceivedQty = item.ReceivedQty,
                AllocatedQty = item.AllocatedQty,
                ConsumedQty = item.ConsumedQty,
                AvailableQty = item.AvailableQty,
                RemainingQty = item.RemainingQty,
                Status = item.Status.ToString()
            };
    }

    public async Task<List<InventorySourceAllocationListItem>> GetAllocationsByReservationIdAsync(Guid reservationRef)
    {
        var items = await _dbContext.Set<InventorySourceAllocationReadModel>()
            .AsNoTracking()
            .Where(x => x.ReservationRef == reservationRef)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToAllocationListItem).ToList();
    }

    public async Task<List<InventorySourceAllocationListItem>> GetAllocationsBySourceBalanceIdAsync(Guid sourceBalanceBusinessKey)
    {
        var items = await _dbContext.Set<InventorySourceAllocationReadModel>()
            .AsNoTracking()
            .Where(x => x.SourceBalanceRef == sourceBalanceBusinessKey)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToAllocationListItem).ToList();
    }

    public async Task<List<InventorySourceConsumptionListItem>> GetConsumptionsByTransactionLineAsync(Guid outboundTransactionLineRef)
    {
        var items = await _dbContext.Set<InventorySourceConsumptionReadModel>()
            .AsNoTracking()
            .Where(x => x.OutboundTransactionLineRef == outboundTransactionLineRef)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToConsumptionListItem).ToList();
    }

    public async Task<List<InventorySourceConsumptionListItem>> GetConsumptionsBySourceBalanceIdAsync(Guid sourceBalanceBusinessKey)
    {
        var items = await _dbContext.Set<InventorySourceConsumptionReadModel>()
            .AsNoTracking()
            .Where(x => x.SourceBalanceRef == sourceBalanceBusinessKey)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToConsumptionListItem).ToList();
    }

    private static InventorySourceBalanceListItem ToBalanceListItem(InventorySourceBalanceReadModel item)
    {
        return new InventorySourceBalanceListItem
        {
            SourceBalanceBusinessKey = item.BusinessKey,
            SourceType = item.SourceType.ToString(),
            VariantRef = item.VariantRef,
            SellerRef = item.SellerRef,
            WarehouseRef = item.WarehouseRef,
            LocationRef = item.LocationRef,
            QualityStatusRef = item.QualityStatusRef,
            LotBatchNo = item.LotBatchNo,
            ReceivedQty = item.ReceivedQty,
            AllocatedQty = item.AllocatedQty,
            ConsumedQty = item.ConsumedQty,
            AvailableQty = item.AvailableQty,
            RemainingQty = item.RemainingQty,
            Status = item.Status.ToString()
        };
    }

    private static InventorySourceAllocationListItem ToAllocationListItem(InventorySourceAllocationReadModel item)
    {
        return new InventorySourceAllocationListItem
        {
            AllocationBusinessKey = item.BusinessKey,
            SourceBalanceRef = item.SourceBalanceRef,
            ReservationRef = item.ReservationRef,
            ReservationAllocationRef = item.ReservationAllocationRef,
            VariantRef = item.VariantRef,
            AllocatedQty = item.AllocatedQty,
            ReleasedQty = item.ReleasedQty,
            ConsumedQty = item.ConsumedQty,
            CreatedAt = item.CreatedAt
        };
    }

    private static InventorySourceConsumptionListItem ToConsumptionListItem(InventorySourceConsumptionReadModel item)
    {
        return new InventorySourceConsumptionListItem
        {
            ConsumptionBusinessKey = item.BusinessKey,
            OutboundTransactionRef = item.OutboundTransactionRef,
            OutboundTransactionLineRef = item.OutboundTransactionLineRef,
            SourceBalanceRef = item.SourceBalanceRef,
            VariantRef = item.VariantRef,
            ConsumedQty = item.ConsumedQty,
            ReasonCode = item.ReasonCode,
            CreatedAt = item.CreatedAt
        };
    }
}
