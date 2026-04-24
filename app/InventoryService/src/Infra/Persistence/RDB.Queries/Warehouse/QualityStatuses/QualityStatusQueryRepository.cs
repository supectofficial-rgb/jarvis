namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.QualityStatuses;

using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.QualityStatuses.Queries.SearchQualityStatuses;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.QualityStatuses.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class QualityStatusQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IQualityStatusQueryRepository
{
    public QualityStatusQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetQualityStatusByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid qualityStatusBusinessKey)
    {
        var item = await _dbContext.Set<QualityStatusReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == qualityStatusBusinessKey);

        return item is null ? null : ToDetail(item);
    }

    public Task<GetQualityStatusByBusinessKeyQueryResult?> GetByIdAsync(Guid qualityStatusId)
        => GetByBusinessKeyAsync(qualityStatusId);

    public async Task<GetQualityStatusByBusinessKeyQueryResult?> GetByCodeAsync(string code)
    {
        var normalized = code.Trim();
        var item = await _dbContext.Set<QualityStatusReadModel>()
            .FirstOrDefaultAsync(x => x.Code == normalized);

        return item is null ? null : ToDetail(item);
    }

    public async Task<SearchQualityStatusesQueryResult> SearchAsync(SearchQualityStatusesQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;

        var qualityStatuses = _dbContext.Set<QualityStatusReadModel>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            var code = query.Code.Trim();
            qualityStatuses = qualityStatuses.Where(x => x.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim();
            qualityStatuses = qualityStatuses.Where(x => x.Name.Contains(name));
        }

        if (query.IsActive.HasValue)
            qualityStatuses = qualityStatuses.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await qualityStatuses.CountAsync();
        var items = await qualityStatuses
            .OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new QualityStatusListItem
            {
                QualityStatusBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new SearchQualityStatusesQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public Task<List<QualityStatusListItem>> GetActiveQualityStatusesAsync()
    {
        return _dbContext.Set<QualityStatusReadModel>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new QualityStatusListItem
            {
                QualityStatusBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public Task<List<QualityStatusLookupItem>> GetLookupAsync(bool includeInactive = false)
    {
        var query = _dbContext.Set<QualityStatusReadModel>().AsQueryable();
        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return query.OrderBy(x => x.Code)
            .Select(x => new QualityStatusLookupItem
            {
                QualityStatusBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync();
    }

    private static GetQualityStatusByBusinessKeyQueryResult ToDetail(QualityStatusReadModel item)
    {
        return new GetQualityStatusByBusinessKeyQueryResult
        {
            QualityStatusBusinessKey = item.BusinessKey,
            Code = item.Code,
            Name = item.Name,
            IsActive = item.IsActive
        };
    }
}
