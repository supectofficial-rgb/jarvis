namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Reservations;

using Insurance.InventoryService.AppCore.Domain.Reservations.Entities;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByBusinessKey;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Reservations.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class InventoryReservationQueryRepository
    : QueryRepository<InventoryServiceQueryDbContext>, IInventoryReservationQueryRepository
{
    public InventoryReservationQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetInventoryReservationByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid reservationBusinessKey)
    {
        var item = await _dbContext.Set<InventoryReservationReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == reservationBusinessKey);

        if (item is null)
            return null;

        var mapped = ToReservationListItem(item);
        return new GetInventoryReservationByBusinessKeyQueryResult
        {
            ReservationBusinessKey = mapped.ReservationBusinessKey,
            OrderRef = mapped.OrderRef,
            OrderItemRef = mapped.OrderItemRef,
            VariantRef = mapped.VariantRef,
            SellerRef = mapped.SellerRef,
            WarehouseRef = mapped.WarehouseRef,
            ChannelRef = mapped.ChannelRef,
            RequestedQuantity = mapped.RequestedQuantity,
            AllocatedQuantity = mapped.AllocatedQuantity,
            ConsumedQuantity = mapped.ConsumedQuantity,
            Status = mapped.Status,
            ExpiresAt = mapped.ExpiresAt
        };
    }

    public async Task<ReservationListItem?> GetByIdAsync(Guid reservationId)
    {
        var item = await _dbContext.Set<InventoryReservationReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == reservationId);

        return item is null ? null : ToReservationListItem(item);
    }

    public async Task<List<ReservationListItem>> GetByOrderAsync(Guid orderRef)
    {
        var items = await _dbContext.Set<InventoryReservationReadModel>()
            .AsNoTracking()
            .Where(x => x.OrderRef == orderRef)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToReservationListItem).ToList();
    }

    public async Task<List<ReservationListItem>> GetByVariantAsync(Guid variantRef)
    {
        var items = await _dbContext.Set<InventoryReservationReadModel>()
            .AsNoTracking()
            .Where(x => x.VariantRef == variantRef)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToReservationListItem).ToList();
    }

    public async Task<List<ReservationListItem>> GetActiveAsync()
    {
        var activeStatuses = new[]
        {
            InventoryReservationStatus.Pending,
            InventoryReservationStatus.Confirmed
        };

        var items = await _dbContext.Set<InventoryReservationReadModel>()
            .AsNoTracking()
            .Where(x => activeStatuses.Contains(x.Status))
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToReservationListItem).ToList();
    }

    public async Task<ReservationListItem?> GetSummaryAsync(Guid reservationBusinessKey)
    {
        var item = await _dbContext.Set<InventoryReservationReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == reservationBusinessKey);

        return item is null ? null : ToReservationListItem(item);
    }

    public async Task<List<ReservationAllocationListItem>> GetAllocationsByReservationIdAsync(Guid reservationBusinessKey)
    {
        var items = await _dbContext.Set<ReservationAllocationReadModel>()
            .AsNoTracking()
            .Where(x => x.ReservationRef == reservationBusinessKey)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToReservationAllocationListItem).ToList();
    }

    public async Task<List<ReservationAllocationListItem>> GetAllocationsByStockDetailIdAsync(Guid stockDetailBusinessKey)
    {
        var items = await _dbContext.Set<ReservationAllocationReadModel>()
            .AsNoTracking()
            .Where(x => x.StockDetailRef == stockDetailBusinessKey)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToReservationAllocationListItem).ToList();
    }

    private static ReservationListItem ToReservationListItem(InventoryReservationReadModel item)
    {
        return new ReservationListItem
        {
            ReservationBusinessKey = item.BusinessKey,
            OrderRef = item.OrderRef,
            OrderItemRef = item.OrderItemRef,
            VariantRef = item.VariantRef,
            SellerRef = item.SellerRef,
            WarehouseRef = item.WarehouseRef,
            ChannelRef = item.ChannelRef,
            RequestedQuantity = item.RequestedQuantity,
            AllocatedQuantity = item.AllocatedQuantity,
            ConsumedQuantity = item.ConsumedQuantity,
            Status = item.Status.ToString(),
            ExpiresAt = item.ExpiresAt
        };
    }

    private static ReservationAllocationListItem ToReservationAllocationListItem(ReservationAllocationReadModel item)
    {
        return new ReservationAllocationListItem
        {
            AllocationBusinessKey = item.BusinessKey,
            ReservationRef = item.ReservationRef,
            StockDetailRef = item.StockDetailRef,
            VariantRef = item.VariantRef,
            WarehouseRef = item.WarehouseRef,
            LocationRef = item.LocationRef,
            QualityStatusRef = item.QualityStatusRef,
            LotBatchNo = item.LotBatchNo,
            SerialRef = item.SerialRef,
            AllocatedQty = item.AllocatedQty,
            ReleasedQty = item.ReleasedQty,
            ConsumedQty = item.ConsumedQty,
            AllocatedAt = item.AllocatedAt
        };
    }
}
