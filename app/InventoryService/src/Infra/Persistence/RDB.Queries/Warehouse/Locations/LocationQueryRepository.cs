namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Locations;

using System.Text.Json;
using Insurance.InventoryService.AppCore.Domain.Warehouse.Entities;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.SearchLocations;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.Common;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.Locations.Entities;
using Insurance.InventoryService.Infra.Persistence.RDB.Queries.Warehouse.LocationStructures.Entities;
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

        if (item is null)
        {
            return null;
        }

        var selections = await _dbContext.Set<LocationStructureSelectionReadModel>()
            .Where(x => x.LocationRef == locationBusinessKey)
            .OrderBy(x => x.Id)
            .Select(x => new LocationStructureSelectionItem
            {
                LocationStructureSelectionBusinessKey = x.BusinessKey,
                LocationRef = x.LocationRef,
                StructureRef = x.StructureRef,
                StructureValueRef = x.StructureValueRef
            })
            .ToListAsync();

        return ToDetail(item, selections);
    }

    public Task<GetLocationByBusinessKeyQueryResult?> GetByIdAsync(Guid locationId)
        => GetByBusinessKeyAsync(locationId);

    public async Task<GetLocationByBusinessKeyQueryResult?> GetByCodeAsync(string locationCode)
    {
        var normalized = locationCode.Trim();
        var item = await _dbContext.Set<LocationReadModel>()
            .FirstOrDefaultAsync(x => x.LocationCode == normalized);

        return item is null ? null : ToDetail(item, Array.Empty<LocationStructureSelectionItem>());
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
                LocationType = x.LocationType.ToString(),
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
        if (!Enum.TryParse<LocationType>(locationType.Trim(), true, out var parsedType))
        {
            return Task.FromResult(new List<LocationListItem>());
        }

        var query = _dbContext.Set<LocationReadModel>()
            .Where(x => x.LocationType == parsedType);

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
                LocationType = x.LocationType.ToString(),
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

        if (!string.IsNullOrWhiteSpace(query.LocationTypes))
        {
            var locationTypes = query.LocationTypes
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim())
                .Select(x => Enum.TryParse<LocationType>(x, true, out var parsedType) ? parsedType : (LocationType?)null)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            if (locationTypes.Count > 0)
            {
                locations = locations.Where(x => locationTypes.Contains(x.LocationType));
            }
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

        var candidates = await locations
            .OrderBy(x => x.LocationCode)
            .Select(x => new LocationListItem
            {
                LocationBusinessKey = x.BusinessKey,
                WarehouseRef = x.WarehouseRef,
                LocationCode = x.LocationCode,
                LocationType = x.LocationType.ToString(),
                Aisle = x.Aisle,
                Rack = x.Rack,
                Shelf = x.Shelf,
                Bin = x.Bin,
                IsActive = x.IsActive
            })
            .ToListAsync();

        var filters = ParseStructureSelectionFilters(query.StructureSelectionsJson);
        var structureSelections = await _dbContext.Set<LocationStructureSelectionReadModel>()
            .Where(x => candidates.Select(c => c.LocationBusinessKey).Contains(x.LocationRef))
            .ToListAsync();

        var structureRefs = structureSelections.Select(x => x.StructureRef).Distinct().ToList();
        var valueRefs = structureSelections.Select(x => x.StructureValueRef).Distinct().ToList();

        var structureNames = await _dbContext.Set<LocationStructureNodeReadModel>()
            .Where(x => structureRefs.Contains(x.BusinessKey))
            .Select(x => new { x.BusinessKey, x.Name })
            .ToDictionaryAsync(x => x.BusinessKey, x => x.Name);

        var valueNames = await _dbContext.Set<LocationStructureValueReadModel>()
            .Where(x => valueRefs.Contains(x.BusinessKey))
            .Select(x => new { x.BusinessKey, x.Name })
            .ToDictionaryAsync(x => x.BusinessKey, x => x.Name);

        var selectionsByLocation = structureSelections
            .GroupBy(x => x.LocationRef)
            .ToDictionary(x => x.Key, x => x.ToList());

        var filtered = filters.Count == 0
            ? candidates
            : candidates.Where(location =>
            {
                var selections = selectionsByLocation.TryGetValue(location.LocationBusinessKey, out var value)
                    ? value
                    : new List<LocationStructureSelectionReadModel>();

                return MatchesSelectionFilters(selections, filters);
            }).ToList();

        foreach (var location in filtered)
        {
            var selections = selectionsByLocation.TryGetValue(location.LocationBusinessKey, out var value)
                ? value
                : new List<LocationStructureSelectionReadModel>();

            location.StructureSummary = BuildStructureSummary(selections, structureNames, valueNames);
        }

        var totalCount = filtered.Count;
        var items = filtered
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

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
                LocationType = x.LocationType.ToString()
            })
            .ToListAsync();
    }

    private GetLocationByBusinessKeyQueryResult ToDetail(LocationReadModel item, IReadOnlyCollection<LocationStructureSelectionItem> selections)
    {
        var structureRefs = selections.Select(x => x.StructureRef).Distinct().ToList();
        var valueRefs = selections.Select(x => x.StructureValueRef).Distinct().ToList();
        var structureNames = _dbContext.Set<LocationStructureNodeReadModel>()
            .Where(x => structureRefs.Contains(x.BusinessKey))
            .ToDictionary(x => x.BusinessKey, x => x.Name);
        var valueNames = _dbContext.Set<LocationStructureValueReadModel>()
            .Where(x => valueRefs.Contains(x.BusinessKey))
            .ToDictionary(x => x.BusinessKey, x => x.Name);

        return new GetLocationByBusinessKeyQueryResult
        {
            LocationBusinessKey = item.BusinessKey,
            WarehouseRef = item.WarehouseRef,
            LocationCode = item.LocationCode,
            LocationType = item.LocationType.ToString(),
            Aisle = item.Aisle,
            Rack = item.Rack,
            Shelf = item.Shelf,
            Bin = item.Bin,
            StructureSummary = BuildStructureSummary(
                selections.Select(x => new LocationStructureSelectionReadModel
                {
                    LocationRef = x.LocationRef,
                    StructureRef = x.StructureRef,
                    StructureValueRef = x.StructureValueRef
                }).ToList(),
                structureNames,
                valueNames),
            StructureSelections = selections.ToList(),
            IsActive = item.IsActive
        };
    }

    private static List<LocationStructureSelectionFilterItem> ParseStructureSelectionFilters(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return new List<LocationStructureSelectionFilterItem>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<LocationStructureSelectionFilterItem>>(json) ?? new List<LocationStructureSelectionFilterItem>();
        }
        catch
        {
            return new List<LocationStructureSelectionFilterItem>();
        }
    }

    private static bool MatchesSelectionFilters(
        IReadOnlyCollection<LocationStructureSelectionReadModel> selections,
        IReadOnlyCollection<LocationStructureSelectionFilterItem> filters)
    {
        if (filters.Count == 0)
        {
            return true;
        }

        foreach (var group in filters.Where(x => x.StructureRef != Guid.Empty))
        {
            var selectedValues = selections
                .Where(x => x.StructureRef == group.StructureRef)
                .Select(x => x.StructureValueRef)
                .ToHashSet();

            if (selectedValues.Count == 0)
            {
                return false;
            }

            if (group.StructureValueRefs.Count > 0 && !group.StructureValueRefs.Any(selectedValues.Contains))
            {
                return false;
            }
        }

        return true;
    }

    private static string BuildStructureSummary(
        IReadOnlyCollection<LocationStructureSelectionReadModel> selections,
        IReadOnlyDictionary<Guid, string> structureNames,
        IReadOnlyDictionary<Guid, string> valueNames)
    {
        if (selections.Count == 0)
        {
            return string.Empty;
        }

        return string.Join(" / ",
            selections
                .Select(x =>
                {
                    var structureName = structureNames.TryGetValue(x.StructureRef, out var sName) ? sName : x.StructureRef.ToString("D");
                    var valueName = valueNames.TryGetValue(x.StructureValueRef, out var vName) ? vName : x.StructureValueRef.ToString("D");
                    return $"{structureName}: {valueName}";
                })
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase));
    }
}

public sealed class LocationStructureSelectionFilterItem
{
    public Guid StructureRef { get; set; }
    public List<Guid> StructureValueRefs { get; set; } = new();
}
