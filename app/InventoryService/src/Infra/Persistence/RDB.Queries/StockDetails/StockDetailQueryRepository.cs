namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetAvailableStockBuckets;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetLowStockBuckets;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetStockAging;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.SearchStockDetails;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.StockDetails.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class StockDetailQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IStockDetailQueryRepository
{
    public StockDetailQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetStockDetailByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid stockDetailBusinessKey)
    {
        var item = await _dbContext.Set<StockDetailReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == stockDetailBusinessKey);

        return item is null ? null : ToBusinessKeyResult(item);
    }

    public async Task<StockDetailListItem?> GetByIdAsync(Guid stockDetailId)
    {
        var item = await _dbContext.Set<StockDetailReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == stockDetailId);

        return item is null ? null : ToListItem(item);
    }

    public async Task<StockDetailListItem?> GetByBucketKeyAsync(Guid variantRef, Guid sellerRef, Guid warehouseRef, Guid locationRef, Guid qualityStatusRef, string? lotBatchNo)
    {
        var normalizedLot = string.IsNullOrWhiteSpace(lotBatchNo) ? null : lotBatchNo.Trim();
        var item = await _dbContext.Set<StockDetailReadModel>()
            .FirstOrDefaultAsync(x =>
                x.VariantRef == variantRef &&
                x.SellerRef == sellerRef &&
                x.WarehouseRef == warehouseRef &&
                x.LocationRef == locationRef &&
                x.QualityStatusRef == qualityStatusRef &&
                x.LotBatchNo == normalizedLot);

        return item is null ? null : ToListItem(item);
    }

    public async Task<SearchStockDetailsQueryResult> SearchAsync(SearchStockDetailsQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : Math.Min(query.PageSize, 200);

        IQueryable<StockDetailReadModel> dbQuery = _dbContext.Set<StockDetailReadModel>();

        if (query.VariantRef.HasValue)
            dbQuery = dbQuery.Where(x => x.VariantRef == query.VariantRef.Value);

        if (query.SellerRef.HasValue)
            dbQuery = dbQuery.Where(x => x.SellerRef == query.SellerRef.Value);

        if (query.WarehouseRef.HasValue)
            dbQuery = dbQuery.Where(x => x.WarehouseRef == query.WarehouseRef.Value);

        if (query.LocationRef.HasValue)
            dbQuery = dbQuery.Where(x => x.LocationRef == query.LocationRef.Value);

        if (query.QualityStatusRef.HasValue)
            dbQuery = dbQuery.Where(x => x.QualityStatusRef == query.QualityStatusRef.Value);

        if (!string.IsNullOrWhiteSpace(query.LotBatchNo))
        {
            var lot = query.LotBatchNo.Trim();
            dbQuery = dbQuery.Where(x => x.LotBatchNo != null && EF.Functions.ILike(x.LotBatchNo, $"%{lot}%"));
        }

        if (query.IsEmpty.HasValue)
            dbQuery = query.IsEmpty.Value ? dbQuery.Where(x => x.QuantityOnHand == 0) : dbQuery.Where(x => x.QuantityOnHand != 0);

        var totalCount = await dbQuery.CountAsync();
        var items = await dbQuery
            .OrderByDescending(x => x.LastUpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new SearchStockDetailsQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items.Select(ToListItem).ToList()
        };
    }

    public async Task<List<StockDetailListItem>> GetByVariantAsync(Guid variantRef)
    {
        var items = await _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.VariantRef == variantRef)
            .OrderByDescending(x => x.LastUpdatedAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<StockDetailListItem>> GetBySellerAsync(Guid sellerRef)
    {
        var items = await _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.SellerRef == sellerRef)
            .OrderByDescending(x => x.LastUpdatedAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<StockDetailListItem>> GetByWarehouseAsync(Guid warehouseRef)
    {
        var items = await _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.WarehouseRef == warehouseRef)
            .OrderByDescending(x => x.LastUpdatedAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<StockDetailListItem>> GetByLocationAsync(Guid locationRef)
    {
        var items = await _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.LocationRef == locationRef)
            .OrderByDescending(x => x.LastUpdatedAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<StockDetailListItem>> GetByQualityStatusAsync(Guid qualityStatusRef)
    {
        var items = await _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.QualityStatusRef == qualityStatusRef)
            .OrderByDescending(x => x.LastUpdatedAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<StockDetailListItem>> GetByLotBatchAsync(string lotBatchNo)
    {
        var normalizedLot = lotBatchNo.Trim();
        var items = await _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.LotBatchNo != null && x.LotBatchNo == normalizedLot)
            .OrderByDescending(x => x.LastUpdatedAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<StockDetailListItem>> GetAvailableBucketsAsync(GetAvailableStockBucketsQuery query)
    {
        IQueryable<StockDetailReadModel> dbQuery = _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.QuantityOnHand > 0);

        if (query.VariantRef.HasValue)
            dbQuery = dbQuery.Where(x => x.VariantRef == query.VariantRef.Value);

        if (query.SellerRef.HasValue)
            dbQuery = dbQuery.Where(x => x.SellerRef == query.SellerRef.Value);

        if (query.WarehouseRef.HasValue)
            dbQuery = dbQuery.Where(x => x.WarehouseRef == query.WarehouseRef.Value);

        if (query.LocationRef.HasValue)
            dbQuery = dbQuery.Where(x => x.LocationRef == query.LocationRef.Value);

        if (query.QualityStatusRef.HasValue)
            dbQuery = dbQuery.Where(x => x.QualityStatusRef == query.QualityStatusRef.Value);

        if (query.MinQuantity.HasValue)
            dbQuery = dbQuery.Where(x => x.QuantityOnHand >= query.MinQuantity.Value);

        var items = await dbQuery
            .OrderByDescending(x => x.LastUpdatedAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<List<StockDetailListItem>> GetEmptyBucketsAsync(Guid? variantRef, Guid? sellerRef, Guid? warehouseRef, Guid? locationRef, Guid? qualityStatusRef)
    {
        IQueryable<StockDetailReadModel> dbQuery = _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.QuantityOnHand == 0);

        if (variantRef.HasValue)
            dbQuery = dbQuery.Where(x => x.VariantRef == variantRef.Value);

        if (sellerRef.HasValue)
            dbQuery = dbQuery.Where(x => x.SellerRef == sellerRef.Value);

        if (warehouseRef.HasValue)
            dbQuery = dbQuery.Where(x => x.WarehouseRef == warehouseRef.Value);

        if (locationRef.HasValue)
            dbQuery = dbQuery.Where(x => x.LocationRef == locationRef.Value);

        if (qualityStatusRef.HasValue)
            dbQuery = dbQuery.Where(x => x.QualityStatusRef == qualityStatusRef.Value);

        var items = await dbQuery
            .OrderByDescending(x => x.LastUpdatedAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    public async Task<VariantStockSummaryItem?> GetVariantSummaryAsync(Guid variantRef)
    {
        var group = await _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.VariantRef == variantRef)
            .GroupBy(x => x.VariantRef)
            .Select(g => new VariantStockSummaryItem
            {
                VariantRef = g.Key,
                TotalQuantityOnHand = g.Sum(x => x.QuantityOnHand),
                BucketCount = g.Count()
            })
            .FirstOrDefaultAsync();

        return group;
    }

    public async Task<WarehouseStockSummaryItem?> GetWarehouseSummaryAsync(Guid warehouseRef)
    {
        var group = await _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.WarehouseRef == warehouseRef)
            .GroupBy(x => x.WarehouseRef)
            .Select(g => new WarehouseStockSummaryItem
            {
                WarehouseRef = g.Key,
                TotalQuantityOnHand = g.Sum(x => x.QuantityOnHand),
                BucketCount = g.Count(),
                VariantCount = g.Select(x => x.VariantRef).Distinct().Count()
            })
            .FirstOrDefaultAsync();

        return group;
    }

    public async Task<SellerStockSummaryItem?> GetSellerSummaryAsync(Guid sellerRef)
    {
        var group = await _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.SellerRef == sellerRef)
            .GroupBy(x => x.SellerRef)
            .Select(g => new SellerStockSummaryItem
            {
                SellerRef = g.Key,
                TotalQuantityOnHand = g.Sum(x => x.QuantityOnHand),
                BucketCount = g.Count(),
                VariantCount = g.Select(x => x.VariantRef).Distinct().Count()
            })
            .FirstOrDefaultAsync();

        return group;
    }

    public async Task<List<StockAgingItem>> GetStockAgingAsync(GetStockAgingQuery query)
    {
        var asOf = query.AsOfUtc ?? DateTime.UtcNow;

        IQueryable<StockDetailReadModel> dbQuery = _dbContext.Set<StockDetailReadModel>();

        if (!query.IncludeEmptyBuckets)
            dbQuery = dbQuery.Where(x => x.QuantityOnHand > 0);

        if (query.VariantRef.HasValue)
            dbQuery = dbQuery.Where(x => x.VariantRef == query.VariantRef.Value);

        if (query.SellerRef.HasValue)
            dbQuery = dbQuery.Where(x => x.SellerRef == query.SellerRef.Value);

        if (query.WarehouseRef.HasValue)
            dbQuery = dbQuery.Where(x => x.WarehouseRef == query.WarehouseRef.Value);

        if (query.LocationRef.HasValue)
            dbQuery = dbQuery.Where(x => x.LocationRef == query.LocationRef.Value);

        if (query.QualityStatusRef.HasValue)
            dbQuery = dbQuery.Where(x => x.QualityStatusRef == query.QualityStatusRef.Value);

        if (!string.IsNullOrWhiteSpace(query.LotBatchNo))
        {
            var lot = query.LotBatchNo.Trim();
            dbQuery = dbQuery.Where(x => x.LotBatchNo == lot);
        }

        var stockDetails = await dbQuery
            .OrderBy(x => x.FirstReceivedAt)
            .ToListAsync();

        var items = stockDetails
            .Select(x =>
            {
                var age = asOf < x.FirstReceivedAt
                    ? 0
                    : (int)Math.Floor((asOf - x.FirstReceivedAt).TotalDays);

                return new StockAgingItem
                {
                    StockDetailBusinessKey = x.BusinessKey,
                    VariantRef = x.VariantRef,
                    SellerRef = x.SellerRef,
                    WarehouseRef = x.WarehouseRef,
                    LocationRef = x.LocationRef,
                    QualityStatusRef = x.QualityStatusRef,
                    LotBatchNo = x.LotBatchNo,
                    QuantityOnHand = x.QuantityOnHand,
                    FirstReceivedAt = x.FirstReceivedAt,
                    LastReceivedAt = x.LastReceivedAt,
                    LastIssuedAt = x.LastIssuedAt,
                    AsOfUtc = asOf,
                    AgeDays = age
                };
            })
            .Where(x => x.AgeDays >= query.MinAgeDays)
            .OrderByDescending(x => x.AgeDays)
            .ToList();

        return items;
    }

    public async Task<List<StockDetailListItem>> GetLowStockBucketsAsync(GetLowStockBucketsQuery query)
    {
        IQueryable<StockDetailReadModel> dbQuery = _dbContext.Set<StockDetailReadModel>()
            .Where(x => x.QuantityOnHand > 0 && x.QuantityOnHand <= query.ThresholdQuantity);

        if (query.VariantRef.HasValue)
            dbQuery = dbQuery.Where(x => x.VariantRef == query.VariantRef.Value);

        if (query.SellerRef.HasValue)
            dbQuery = dbQuery.Where(x => x.SellerRef == query.SellerRef.Value);

        if (query.WarehouseRef.HasValue)
            dbQuery = dbQuery.Where(x => x.WarehouseRef == query.WarehouseRef.Value);

        if (query.LocationRef.HasValue)
            dbQuery = dbQuery.Where(x => x.LocationRef == query.LocationRef.Value);

        if (query.QualityStatusRef.HasValue)
            dbQuery = dbQuery.Where(x => x.QualityStatusRef == query.QualityStatusRef.Value);

        var items = await dbQuery
            .OrderBy(x => x.QuantityOnHand)
            .ThenByDescending(x => x.LastUpdatedAt)
            .ToListAsync();

        return items.Select(ToListItem).ToList();
    }

    private static StockDetailListItem ToListItem(StockDetailReadModel item)
        => new()
        {
            StockDetailBusinessKey = item.BusinessKey,
            VariantRef = item.VariantRef,
            SellerRef = item.SellerRef,
            WarehouseRef = item.WarehouseRef,
            LocationRef = item.LocationRef,
            QualityStatusRef = item.QualityStatusRef,
            LotBatchNo = item.LotBatchNo,
            QuantityOnHand = item.QuantityOnHand,
            FirstReceivedAt = item.FirstReceivedAt,
            LastReceivedAt = item.LastReceivedAt,
            LastIssuedAt = item.LastIssuedAt,
            LastUpdatedAt = item.LastUpdatedAt
        };

    private static GetStockDetailByBusinessKeyQueryResult ToBusinessKeyResult(StockDetailReadModel item)
        => new()
        {
            StockDetailBusinessKey = item.BusinessKey,
            VariantRef = item.VariantRef,
            SellerRef = item.SellerRef,
            WarehouseRef = item.WarehouseRef,
            LocationRef = item.LocationRef,
            QualityStatusRef = item.QualityStatusRef,
            LotBatchNo = item.LotBatchNo,
            QuantityOnHand = item.QuantityOnHand,
            FirstReceivedAt = item.FirstReceivedAt,
            LastReceivedAt = item.LastReceivedAt,
            LastIssuedAt = item.LastIssuedAt,
            LastUpdatedAt = item.LastUpdatedAt
        };
}
