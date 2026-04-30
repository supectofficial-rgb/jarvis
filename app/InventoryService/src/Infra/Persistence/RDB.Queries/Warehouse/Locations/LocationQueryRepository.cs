namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Locations;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.SearchLocations;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Locations.Entities;
using Microsoft.EntityFrameworkCore;
using OysterFx.Infra.Persistence.RDB.Queries;

public class LocationQueryRepository : QueryRepository<InventoryServiceQueryDbContext>, ILocationQueryRepository
{
    public LocationQueryRepository(InventoryServiceQueryDbContext dbContext)
        : base(dbContext)
    {
    }

    public async Task<GetLocationByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid locationBusinessKey)
    {
        var item = await _dbContext.Set<LocationReadModel>()
            .FirstOrDefaultAsync(x => x.BusinessKey == locationBusinessKey);

        return item is null ? null : ToDetail(item);
    }

    public Task<GetLocationByBusinessKeyQueryResult?> GetByIdAsync(Guid locationId)
        => GetByBusinessKeyAsync(locationId);

    public async Task<GetLocationByBusinessKeyQueryResult?> GetByCodeAsync(string locationCode)
    {
        var normalized = locationCode.Trim();
        var item = await _dbContext.Set<LocationReadModel>()
            .FirstOrDefaultAsync(x => x.LocationCode == normalized);

        return item is null ? null : ToDetail(item);
    }

    public Task<List<LocationListItem>> GetByWarehouseIdAsync(Guid warehouseId, bool onlyActive = false)
    {
        var query = _dbContext.Set<LocationReadModel>()
            .Where(x => x.WarehouseRef == warehouseId);

        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.OrderBy(x => x.LocationCode)
            .Select(x => new LocationListItem
            {
                LocationBusinessKey = x.BusinessKey,
                WarehouseRef = x.WarehouseRef,
                LocationCode = x.LocationCode,
                LocationType = x.LocationType,
                Aisle = x.Aisle,
                Rack = x.Rack,
                Shelf = x.Shelf,
                Bin = x.Bin,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public Task<List<LocationListItem>> GetByTypeAsync(string locationType, Guid? warehouseRef = null, bool onlyActive = false)
    {
        var normalizedType = locationType.Trim();

        var query = _dbContext.Set<LocationReadModel>()
            .Where(x => x.LocationType == normalizedType);

        if (warehouseRef.HasValue)
            query = query.Where(x => x.WarehouseRef == warehouseRef.Value);

        if (onlyActive)
            query = query.Where(x => x.IsActive);

        return query.OrderBy(x => x.LocationCode)
            .Select(x => new LocationListItem
            {
                LocationBusinessKey = x.BusinessKey,
                WarehouseRef = x.WarehouseRef,
                LocationCode = x.LocationCode,
                LocationType = x.LocationType,
                Aisle = x.Aisle,
                Rack = x.Rack,
                Shelf = x.Shelf,
                Bin = x.Bin,
                IsActive = x.IsActive
            })
            .ToListAsync();
    }

    public async Task<SearchLocationsQueryResult> SearchAsync(SearchLocationsQuery query)
    {
        var page = query.Page <= 0 ? 1 : query.Page;
        var pageSize = query.PageSize <= 0 ? 20 : query.PageSize;
        var locations = _dbContext.Set<LocationReadModel>().AsQueryable();

        if (query.WarehouseRef.HasValue)
            locations = locations.Where(x => x.WarehouseRef == query.WarehouseRef.Value);

        if (!string.IsNullOrWhiteSpace(query.LocationCode))
        {
            var code = query.LocationCode.Trim();
            locations = locations.Where(x => x.LocationCode.Contains(code));
        }

        if (!string.IsNullOrWhiteSpace(query.LocationType))
        {
            var locationType = query.LocationType.Trim();
            locations = locations.Where(x => x.LocationType == locationType);
        }

        if (!string.IsNullOrWhiteSpace(query.Aisle))
        {
            var aisle = query.Aisle.Trim();
            locations = locations.Where(x => x.Aisle != null && x.Aisle.Contains(aisle));
        }

        if (!string.IsNullOrWhiteSpace(query.Rack))
        {
            var rack = query.Rack.Trim();
            locations = locations.Where(x => x.Rack != null && x.Rack.Contains(rack));
        }

        if (!string.IsNullOrWhiteSpace(query.Shelf))
        {
            var shelf = query.Shelf.Trim();
            locations = locations.Where(x => x.Shelf != null && x.Shelf.Contains(shelf));
        }

        if (!string.IsNullOrWhiteSpace(query.Bin))
        {
            var bin = query.Bin.Trim();
            locations = locations.Where(x => x.Bin != null && x.Bin.Contains(bin));
        }

        if (query.IsActive.HasValue)
            locations = locations.Where(x => x.IsActive == query.IsActive.Value);

        var totalCount = await locations.CountAsync();
        var items = await locations
            .OrderBy(x => x.LocationCode)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new LocationListItem
            {
                LocationBusinessKey = x.BusinessKey,
                WarehouseRef = x.WarehouseRef,
                LocationCode = x.LocationCode,
                LocationType = x.LocationType,
                Aisle = x.Aisle,
                Rack = x.Rack,
                Shelf = x.Shelf,
                Bin = x.Bin,
                IsActive = x.IsActive
            })
            .ToListAsync();

        return new SearchLocationsQueryResult
        {
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            Items = items
        };
    }

    public Task<List<LocationLookupItem>> GetLookupAsync(Guid? warehouseRef = null, bool includeInactive = false)
    {
        var query = _dbContext.Set<LocationReadModel>().AsQueryable();

        if (warehouseRef.HasValue)
            query = query.Where(x => x.WarehouseRef == warehouseRef.Value);

        if (!includeInactive)
            query = query.Where(x => x.IsActive);

        return query.OrderBy(x => x.LocationCode)
            .Select(x => new LocationLookupItem
            {
                LocationBusinessKey = x.BusinessKey,
                WarehouseRef = x.WarehouseRef,
                LocationCode = x.LocationCode,
                LocationType = x.LocationType
            })
            .ToListAsync();
    }

    private static GetLocationByBusinessKeyQueryResult ToDetail(LocationReadModel item)
    {
        return new GetLocationByBusinessKeyQueryResult
        {
            LocationBusinessKey = item.BusinessKey,
            WarehouseRef = item.WarehouseRef,
            LocationCode = item.LocationCode,
            LocationType = item.LocationType,
            Aisle = item.Aisle,
            Rack = item.Rack,
            Shelf = item.Shelf,
            Bin = item.Bin,
            IsActive = item.IsActive
        };
    }
}
