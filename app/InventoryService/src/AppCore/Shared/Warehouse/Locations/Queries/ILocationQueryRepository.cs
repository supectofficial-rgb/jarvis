namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Locations.Queries.SearchLocations;
using OysterFx.AppCore.Shared.Queries;

public interface ILocationQueryRepository : IQueryRepository
{
    Task<GetLocationByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid locationBusinessKey);
    Task<GetLocationByBusinessKeyQueryResult?> GetByIdAsync(Guid locationId);
    Task<GetLocationByBusinessKeyQueryResult?> GetByCodeAsync(string locationCode);
    Task<List<LocationListItem>> GetByWarehouseIdAsync(Guid warehouseId, bool onlyActive = false);
    Task<List<LocationListItem>> GetByTypeAsync(string locationType, Guid? warehouseRef = null, bool onlyActive = false);
    Task<SearchLocationsQueryResult> SearchAsync(SearchLocationsQuery query);
    Task<List<LocationLookupItem>> GetLookupAsync(Guid? warehouseRef = null, bool includeInactive = false);
}
