namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.SerialItems;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.SearchSerialItems;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.SerialItems.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class SerialItemQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, ISerialItemQueryRepository
{
    public SerialItemQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetSerialItemByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid serialItemBusinessKey)
    {
        var item = await _dbContext.Set<SerialItemReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == serialItemBusinessKey);

        return item is null ? null : ToBusinessKeyResult(item);
    }

    public async Task<SerialItemListItem?> GetByIdAsync(Guid serialItemId)
    {
        var item = await _dbContext.Set<SerialItemReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == serialItemId);

        return item is null ? null : ToListItem(item);
    }

    public async Task<SerialItemListItem?> GetBySerialNoAsync(string serialNo, Guid? variantRef = null)
    {
        if (string.IsNullOrWhiteSpace(serialNo))
            return null;

        var normalized = serialNo.Trim();
        var query = _dbContext.Set<SerialItemReadModel>().Where(x => x.SerialNo == normalized);
        if (variantRef.HasValue)
            query = query.Where(x => x.VariantRef == variantRef.Value);

        var item = await query.FirstOrDefaultAsync();
        return item is null ? null : ToListItem(item);
    }

    public async Task<SearchSerialItemsQueryResult> SearchAsync(SearchSerialItemsQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 200);

        IQueryable<SerialItemReadModel> dbQuery = _dbContext.Set<SerialItemReadModel>();

        if (!string.IsNullOrWhiteSpace(query.SerialNo))
        {
            var serialNo = query.SerialNo.Trim();
            dbQuery = dbQuery.Where(x => EF.Functions.ILike(x.SerialNo, $"%{serialNo}%"));
        }

        if (query.VariantRef.HasValue)
            dbQuery = dbQuery.Where(x => x.VariantRef == query.VariantRef.Value);

        if (query.SellerRef.HasValue)
            dbQuery = dbQuery.Where(x => x.SellerRef == query.SellerRef.Value);

        if (query.WarehouseRef.HasValue)
            dbQuery = dbQuery.Where(x => x.WarehouseRef == query.WarehouseRef.Value);

        if (query.LocationRef.HasValue)
            dbQuery = dbQuery.Where(x => x.LocationRef == query.LocationRef.Value);

        if (query.StockDetailRef.HasValue)
            dbQuery = dbQuery.Where(x => x.StockDetailRef == query.StockDetailRef.Value);

        if (query.QualityStatusRef.HasValue)
            dbQuery = dbQuery.Where(x => x.QualityStatusRef == query.QualityStatusRef.Value);

        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<Insurance.InventoryService.AppCore.Domain.StockDetails.Entities.SerialItemStatus>(query.Status, true, out var status))
            dbQuery = dbQuery.Where(x => x.Status == status);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .OrderBy(x => x.SerialNo)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new SearchSerialItemsQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items.Select(ToListItem).ToList()
        };
    }

    public async Task<List<SerialItemListItem>> GetByVariantAsync(Guid variantRef)
    {
        var items = await _dbContext.Set<SerialItemReadModel>()
            .Where(x => x.VariantRef == variantRef)
            .OrderBy(x => x.SerialNo)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<SerialItemListItem>> GetAvailableAsync(Guid? variantRef = null, Guid? warehouseRef = null)
    {
        var query = _dbContext.Set<SerialItemReadModel>()
            .Where(x => x.Status == Insurance.InventoryService.AppCore.Domain.StockDetails.Entities.SerialItemStatus.Available);

        if (variantRef.HasValue)
            query = query.Where(x => x.VariantRef == variantRef.Value);

        if (warehouseRef.HasValue)
            query = query.Where(x => x.WarehouseRef == warehouseRef.Value);

        var items = await query.OrderBy(x => x.SerialNo).ToListAsync();
        return items.Select(ToListItem).ToList();
    }

    private static SerialItemListItem ToListItem(SerialItemReadModel x)
        => new()
        {
            SerialItemBusinessKey = x.BusinessKey,
            SerialNo = x.SerialNo,
            VariantRef = x.VariantRef,
            SellerRef = x.SellerRef,
            WarehouseRef = x.WarehouseRef,
            LocationRef = x.LocationRef,
            StockDetailRef = x.StockDetailRef,
            QualityStatusRef = x.QualityStatusRef,
            LotBatchNo = x.LotBatchNo,
            Status = x.Status.ToString(),
            DateScannedIn = x.DateScannedIn,
            LastTransactionRef = x.LastTransactionRef,
            LastUpdatedAt = x.LastUpdatedAt
        };

    private static GetSerialItemByBusinessKeyQueryResult ToBusinessKeyResult(SerialItemReadModel x)
        => new()
        {
            SerialItemBusinessKey = x.BusinessKey,
            SerialNo = x.SerialNo,
            VariantRef = x.VariantRef,
            SellerRef = x.SellerRef,
            WarehouseRef = x.WarehouseRef,
            LocationRef = x.LocationRef,
            StockDetailRef = x.StockDetailRef,
            QualityStatusRef = x.QualityStatusRef,
            LotBatchNo = x.LotBatchNo,
            Status = x.Status.ToString(),
            DateScannedIn = x.DateScannedIn,
            LastTransactionRef = x.LastTransactionRef,
            LastUpdatedAt = x.LastUpdatedAt
        };
}
