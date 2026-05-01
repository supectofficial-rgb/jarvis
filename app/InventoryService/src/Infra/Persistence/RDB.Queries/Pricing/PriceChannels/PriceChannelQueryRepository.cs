namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceChannels;

using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Pricing.PriceChannels.Queries.SearchPriceChannels;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Pricing.PriceChannels.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class PriceChannelQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IPriceChannelQueryRepository
{
    public PriceChannelQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetPriceChannelByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid priceChannelBusinessKey)
    {
        var item = await _dbContext.Set<PriceChannelReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == priceChannelBusinessKey);

        return item is null ? null : ToDetail(item);
    }

    public async Task<SearchPriceChannelsQueryResult> SearchAsync(SearchPriceChannelsQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 50 : query.PageSize;
        var itemsQuery = _dbContext.Set<PriceChannelReadModel>().AsQueryable();

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
            .Select(x => new PriceChannelListItem
            {
                PriceChannelBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new SearchPriceChannelsQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public Task<List<PriceChannelListItem>> GetActiveAsync()
    {
        return _dbContext.Set<PriceChannelReadModel>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new PriceChannelListItem
            {
                PriceChannelBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public Task<List<PriceChannelLookupItem>> GetLookupAsync(bool includeInactive = false)
    {
        var query = _dbContext.Set<PriceChannelReadModel>().AsQueryable();
        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return query
            .OrderBy(x => x.Code)
            .Select(x => new PriceChannelLookupItem
            {
                PriceChannelBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    private static GetPriceChannelByBusinessKeyQueryResult ToDetail(PriceChannelReadModel item)
    {
        return new GetPriceChannelByBusinessKeyQueryResult
        {
            PriceChannelBusinessKey = item.BusinessKey,
            Code = item.Code,
            Name = item.Name,
            IsActive = item.IsActive
        };
    }
}
