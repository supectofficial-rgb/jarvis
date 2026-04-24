namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.UnitOfMeasures;

using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Catalog.UnitOfMeasures.Queries.SearchUnitOfMeasures;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Catalog.UnitOfMeasures.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class UnitOfMeasureQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IUnitOfMeasureQueryRepository
{
    public UnitOfMeasureQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetUnitOfMeasureByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid unitOfMeasureBusinessKey)
    {
        var aggregate = await _dbContext.Set<UnitOfMeasureReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == unitOfMeasureBusinessKey);

        return aggregate is null ? null : ToDetail(aggregate);
    }

    public Task<GetUnitOfMeasureByBusinessKeyQueryResult?> GetByIdAsync(Guid unitOfMeasureId)
        => GetByBusinessKeyAsync(unitOfMeasureId);

    public async Task<GetUnitOfMeasureByBusinessKeyQueryResult?> GetByCodeAsync(string code)
    {
        var normalized = code.Trim();
        var aggregate = await _dbContext.Set<UnitOfMeasureReadModel>()
            .FirstOrDefaultAsync(x => x.Code == normalized);

        return aggregate is null ? null : ToDetail(aggregate);
    }

    public async Task<SearchUnitOfMeasuresQueryResult> SearchAsync(SearchUnitOfMeasuresQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        var unitOfMeasures = _dbContext.Set<UnitOfMeasureReadModel>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            var code = query.Code.Trim();
            unitOfMeasures = unitOfMeasures.Where(x => x.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim();
            unitOfMeasures = unitOfMeasures.Where(x => x.Name.Contains(name));
        }

        if (query.IsActive.HasValue)
            unitOfMeasures = unitOfMeasures.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await unitOfMeasures.CountAsync();
        var items = await unitOfMeasures
            .OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new UnitOfMeasureListItem
            {
                UnitOfMeasureBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                Precision = x.Precision,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new SearchUnitOfMeasuresQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public Task<List<UnitOfMeasureListItem>> GetActiveUnitOfMeasuresAsync()
    {
        return _dbContext.Set<UnitOfMeasureReadModel>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new UnitOfMeasureListItem
            {
                UnitOfMeasureBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                Precision = x.Precision,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public Task<List<UnitOfMeasureLookupItem>> GetLookupAsync(bool includeInactive = false)
    {
        var query = _dbContext.Set<UnitOfMeasureReadModel>().AsQueryable();
        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return query.OrderBy(x => x.Code)
            .Select(x => new UnitOfMeasureLookupItem
            {
                UnitOfMeasureBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync();
    }

    private static GetUnitOfMeasureByBusinessKeyQueryResult ToDetail(UnitOfMeasureReadModel item)
    {
        return new GetUnitOfMeasureByBusinessKeyQueryResult
        {
            UnitOfMeasureBusinessKey = item.BusinessKey,
            Code = item.Code,
            Name = item.Name,
            Precision = item.Precision,
            IsActive = item.IsActive
        };
    }
}
