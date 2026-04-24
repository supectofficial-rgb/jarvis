namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Warehouses;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.SearchWarehouses;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Locations.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Warehouses.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class WarehouseQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, IWarehouseQueryRepository
{
    public WarehouseQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetWarehouseByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid warehouseBusinessKey)
    {
        var item = await _dbContext.Set<WarehouseReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == warehouseBusinessKey);

        return item is null ? null : ToDetail(item);
    }

    public Task<GetWarehouseByBusinessKeyQueryResult?> GetByIdAsync(Guid warehouseId)
        => GetByBusinessKeyAsync(warehouseId);

    public async Task<GetWarehouseByBusinessKeyQueryResult?> GetByCodeAsync(string code)
    {
        var normalized = code.Trim();
        var item = await _dbContext.Set<WarehouseReadModel>()
            .FirstOrDefaultAsync(x => x.Code == normalized);

        return item is null ? null : ToDetail(item);
    }

    public async Task<SearchWarehousesQueryResult> SearchAsync(SearchWarehousesQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;

        var warehouses = _dbContext.Set<WarehouseReadModel>().AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Code))
        {
            var code = query.Code.Trim();
            warehouses = warehouses.Where(x => x.Code.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(query.Name))
        {
            var name = query.Name.Trim();
            warehouses = warehouses.Where(x => x.Name.Contains(name));
        }

        if (query.IsActive.HasValue)
            warehouses = warehouses.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await warehouses.CountAsync();
        var items = await warehouses
            .OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new WarehouseListItem
            {
                WarehouseBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new SearchWarehousesQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public Task<List<WarehouseListItem>> GetActiveWarehousesAsync()
    {
        return _dbContext.Set<WarehouseReadModel>()
            .Where(x => x.IsActive)
            .OrderBy(x => x.Code)
            .Select(x => new WarehouseListItem
            {
                WarehouseBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public Task<List<WarehouseLookupItem>> GetLookupAsync(bool includeInactive = false)
    {
        var query = _dbContext.Set<WarehouseReadModel>().AsQueryable();
        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return query.OrderBy(x => x.Code)
            .Select(x => new WarehouseLookupItem
            {
                WarehouseBusinessKey = x.BusinessKey,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync();
    }

    public async Task<WarehouseSummaryItem?> GetSummaryAsync(Guid warehouseBusinessKey)
    {
        var warehouse = await _dbContext.Set<WarehouseReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == warehouseBusinessKey);

        if (warehouse is null)
            return null;

        var locations = _dbContext.Set<LocationReadModel>()
            .Where(x => x.WarehouseRef == warehouseBusinessKey);

        var locationCount = await locations.CountAsync();
        var activeLocationCount = await locations.CountAsync(x => x.IsActive);

        return new WarehouseSummaryItem
        {
            WarehouseBusinessKey = warehouse.BusinessKey,
            Code = warehouse.Code,
            Name = warehouse.Name,
            IsActive = warehouse.IsActive,
            LocationCount = locationCount,
            ActiveLocationCount = activeLocationCount
        };
    }

    public async Task<WarehouseWithLocationsItem?> GetWithLocationsAsync(Guid warehouseBusinessKey, bool includeInactiveLocations = false)
    {
        var warehouse = await _dbContext.Set<WarehouseReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == warehouseBusinessKey);

        if (warehouse is null)
            return null;

        var locationQuery = _dbContext.Set<LocationReadModel>()
            .Where(x => x.WarehouseRef == warehouseBusinessKey);

        if (!includeInactiveLocations)
            locationQuery = locationQuery.Where(x => x.IsActive);

        var locations = await locationQuery
            .OrderBy(x => x.LocationCode)
            .Select(x => new WarehouseLocationItem
            {
                LocationBusinessKey = x.BusinessKey,
                LocationCode = x.LocationCode,
                LocationType = x.LocationType,
                Aisle = x.Aisle,
                Rack = x.Rack,
                Shelf = x.Shelf,
                Bin = x.Bin,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new WarehouseWithLocationsItem
        {
            WarehouseBusinessKey = warehouse.BusinessKey,
            Code = warehouse.Code,
            Name = warehouse.Name,
            IsActive = warehouse.IsActive,
            Locations = locations
        };
    }

    private static GetWarehouseByBusinessKeyQueryResult ToDetail(WarehouseReadModel item)
    {
        return new GetWarehouseByBusinessKeyQueryResult
        {
            WarehouseBusinessKey = item.BusinessKey,
            Code = item.Code,
            Name = item.Name,
            IsActive = item.IsActive
        };
    }
}
