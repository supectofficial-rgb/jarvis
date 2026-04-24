namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Seller.Sellers;

using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Seller.Sellers.Queries.SearchSellers;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Seller.Sellers.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class SellerQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, ISellerQueryRepository
{
    public SellerQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetSellerByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid sellerBusinessKey)
    {
        var item = await _dbContext.Set<SellerReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == sellerBusinessKey);

        return item is null ? null : ToDetail(item);
    }

    public Task<GetSellerByBusinessKeyQueryResult?> GetByIdAsync(Guid sellerId)
        => GetByBusinessKeyAsync(sellerId);

    public async Task<GetSellerByBusinessKeyQueryResult?> GetByCodeAsync(string code)
    {
        var normalized = code.Trim();
        var item = await _dbContext.Set<SellerReadModel>()
            .FirstOrDefaultAsync(x => x.Code == normalized);

        return item is null ? null : ToDetail(item);
    }

    public async Task<SearchSellersQueryResult> SearchAsync(SearchSellersQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;

        var sellers = _dbContext.Set<SellerReadModel>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            var code = query.Code.Trim();
            sellers = sellers.Where(x => x.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim();
            sellers = sellers.Where(x => x.Name.Contains(name));
        }

        if (query.IsSystemOwner.HasValue)
            sellers = sellers.Where(x => x.IsSystemOwner == query.IsSystemOwner.Value);

        if (query.IsActive.HasValue)
            sellers = sellers.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await sellers.CountAsync();
        var items = await sellers
            .OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SellerListItem
            {
                SellerBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsSystemOwner = x.IsSystemOwner,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new SearchSellersQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public Task<List<SellerListItem>> GetActiveSellersAsync()
    {
        return _dbContext.Set<SellerReadModel>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new SellerListItem
            {
                SellerBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsSystemOwner = x.IsSystemOwner,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public Task<List<SellerLookupItem>> GetLookupAsync(bool includeInactive = false)
    {
        var query = _dbContext.Set<SellerReadModel>().AsQueryable();
        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return query.OrderBy(x => x.Code)
            .Select(x => new SellerLookupItem
            {
                SellerBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync();
    }

    public async Task<SellerSummaryItem?> GetSummaryAsync(Guid sellerBusinessKey)
    {
        var item = await _dbContext.Set<SellerReadModel>()
            .Where(x => x.BusinessKey == sellerBusinessKey)
            .Select(x => new SellerSummaryItem
            {
                SellerBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsSystemOwner = x.IsSystemOwner,
                IsActive = x.IsActive
            })
            .FirstOrDefaultAsync();

        return item;
    }

    private static GetSellerByBusinessKeyQueryResult ToDetail(SellerReadModel item)
    {
        return new GetSellerByBusinessKeyQueryResult
        {
            SellerBusinessKey = item.BusinessKey,
            Code = item.Code,
            Name = item.Name,
            IsSystemOwner = item.IsSystemOwner,
            IsActive = item.IsActive
        };
    }
}
