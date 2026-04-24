namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Fulfillments;

using Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByBusinessKey;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Fulfillments.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class FulfillmentQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IFulfillmentQueryRepository
{
    public FulfillmentQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetFulfillmentByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid fulfillmentBusinessKey)
    {
        var item = await _dbContext.Set<FulfillmentReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == fulfillmentBusinessKey);

        if (item is null)
            return null;

        var mapped = ToListItem(item);
        return new GetFulfillmentByBusinessKeyQueryResult
        {
            FulfillmentBusinessKey = mapped.FulfillmentBusinessKey,
            OrderRef = mapped.OrderRef,
            SellerRef = mapped.SellerRef,
            WarehouseRef = mapped.WarehouseRef,
            ChannelRef = mapped.ChannelRef,
            Status = mapped.Status,
            PickedAt = mapped.PickedAt,
            PackedAt = mapped.PackedAt,
            ShippedAt = mapped.ShippedAt
        };
    }

    public async Task<FulfillmentListItem?> GetByIdAsync(Guid fulfillmentId)
    {
        var item = await _dbContext.Set<FulfillmentReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == fulfillmentId);

        return item is null ? null : ToListItem(item);
    }

    public async Task<List<FulfillmentListItem>> GetByOrderAsync(Guid orderRef)
    {
        var items = await _dbContext.Set<FulfillmentReadModel>()
            .AsNoTracking()
            .Where(x => x.OrderRef == orderRef)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<FulfillmentListItem>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<FulfillmentStatus>(status, true, out var parsedStatus))
            return new List<FulfillmentListItem>();

        var items = await _dbContext.Set<FulfillmentReadModel>()
            .AsNoTracking()
            .Where(x => x.Status == parsedStatus)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<FulfillmentListItem?> GetSummaryAsync(Guid fulfillmentBusinessKey)
    {
        var item = await _dbContext.Set<FulfillmentReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == fulfillmentBusinessKey);

        return item is null ? null : ToListItem(item);
    }

    private static FulfillmentListItem ToListItem(FulfillmentReadModel item)
    {
        return new FulfillmentListItem
        {
            FulfillmentBusinessKey = item.BusinessKey,
            OrderRef = item.OrderRef,
            SellerRef = item.SellerRef,
            WarehouseRef = item.WarehouseRef,
            ChannelRef = item.ChannelRef,
            Status = item.Status.ToString(),
            PickedAt = item.PickedAt,
            PackedAt = item.PackedAt,
            ShippedAt = item.ShippedAt
        };
    }
}
