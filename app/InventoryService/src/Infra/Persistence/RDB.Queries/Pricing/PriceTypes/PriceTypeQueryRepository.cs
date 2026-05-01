namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceTypes;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceTypes.Queries.SearchPriceTypes;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceTypes.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class PriceTypeQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IPriceTypeQueryRepository
{
    public PriceTypeQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetPriceTypeByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid priceTypeBusinessKey)
    {
        var item = await _dbContext.Set<PriceTypeReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == priceTypeBusinessKey);

        return item is null ? null : ToDetail(item);
    }

    public async Task<SearchPriceTypesQueryResult> SearchAsync(SearchPriceTypesQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 50 : query.PageSize;
        var itemsQuery = _dbContext.Set<PriceTypeReadModel>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            var code = query.Code.Trim();
            itemsQuery = itemsQuery.Where(x => x.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim();
            itemsQuery = itemsQuery.Where(x => x.Name.Contains(name));
        }

        if (query.IsActive.HasValue)
            itemsQuery = itemsQuery.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await itemsQuery.CountAsync();
        var items = await itemsQuery
            .OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new PriceTypeListItem
            {
                PriceTypeBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new SearchPriceTypesQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public Task<List<PriceTypeListItem>> GetActiveAsync()
    {
        return _dbContext.Set<PriceTypeReadModel>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new PriceTypeListItem
            {
                PriceTypeBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public Task<List<PriceTypeLookupItem>> GetLookupAsync(bool includeInactive = false)
    {
        var query = _dbContext.Set<PriceTypeReadModel>().AsQueryable();
        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return query
            .OrderBy(x => x.Code)
            .Select(x => new PriceTypeLookupItem
            {
                PriceTypeBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    private static GetPriceTypeByBusinessKeyQueryResult ToDetail(PriceTypeReadModel item)
    {
        return new GetPriceTypeByBusinessKeyQueryResult
        {
            PriceTypeBusinessKey = item.BusinessKey,
            Code = item.Code,
            Name = item.Name,
            IsActive = item.IsActive
        };
    }
}
