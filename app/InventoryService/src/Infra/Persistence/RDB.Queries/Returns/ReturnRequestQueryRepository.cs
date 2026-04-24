namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Returns;

using Insurance.InventoryService.AppCore.Domain.Returns.Entities;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByBusinessKey;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Returns.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class ReturnRequestQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IReturnRequestQueryRepository
{
    public ReturnRequestQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetReturnRequestByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid returnRequestBusinessKey)
    {
        var item = await _dbContext.Set<ReturnRequestReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == returnRequestBusinessKey);

        if (item is null)
            return null;

        var mapped = ToListItem(item);
        return new GetReturnRequestByBusinessKeyQueryResult
        {
            ReturnRequestBusinessKey = mapped.ReturnRequestBusinessKey,
            OrderRef = mapped.OrderRef,
            OrderItemRef = mapped.OrderItemRef,
            SellerRef = mapped.SellerRef,
            WarehouseRef = mapped.WarehouseRef,
            Status = mapped.Status,
            ReasonCode = mapped.ReasonCode,
            RequestedAt = mapped.RequestedAt,
            ApprovedAt = mapped.ApprovedAt,
            ReceivedAt = mapped.ReceivedAt
        };
    }

    public async Task<ReturnRequestListItem?> GetByIdAsync(Guid returnRequestId)
    {
        var item = await _dbContext.Set<ReturnRequestReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == returnRequestId);

        return item is null ? null : ToListItem(item);
    }

    public async Task<List<ReturnRequestListItem>> GetByOrderAsync(Guid orderRef)
    {
        var items = await _dbContext.Set<ReturnRequestReadModel>()
            .AsNoTracking()
            .Where(x => x.OrderRef == orderRef)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<ReturnRequestListItem>> GetByStatusAsync(string status)
    {
        if (!Enum.TryParse<ReturnRequestStatus>(status, true, out var parsedStatus))
            return new List<ReturnRequestListItem>();

        var items = await _dbContext.Set<ReturnRequestReadModel>()
            .AsNoTracking()
            .Where(x => x.Status == parsedStatus)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<ReturnRequestListItem?> GetSummaryAsync(Guid returnRequestBusinessKey)
    {
        var item = await _dbContext.Set<ReturnRequestReadModel>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.BusinessKey == returnRequestBusinessKey);

        return item is null ? null : ToListItem(item);
    }

    private static ReturnRequestListItem ToListItem(ReturnRequestReadModel item)
    {
        return new ReturnRequestListItem
        {
            ReturnRequestBusinessKey = item.BusinessKey,
            OrderRef = item.OrderRef,
            OrderItemRef = item.OrderItemRef,
            SellerRef = item.SellerRef,
            WarehouseRef = item.WarehouseRef,
            Status = item.Status.ToString(),
            ReasonCode = item.ReasonCode,
            RequestedAt = item.RequestedAt,
            ApprovedAt = item.ApprovedAt,
            ReceivedAt = item.ReceivedAt
        };
    }
}
