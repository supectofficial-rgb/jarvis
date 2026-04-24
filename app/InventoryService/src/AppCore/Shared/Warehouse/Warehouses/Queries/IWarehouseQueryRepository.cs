namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries;

using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.Common;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetByBusinessKey;
using Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.SearchWarehouses;
using OysterFx.AppCore.Shared.Queries;

public interface IWarehouseQueryRepository : IQueryRepository
{
    Task<GetWarehouseByBusinessKeyQueryResult?> GetByBusinessKeyAsync(Guid warehouseBusinessKey);
    Task<GetWarehouseByBusinessKeyQueryResult?> GetByIdAsync(Guid warehouseId);
    Task<GetWarehouseByBusinessKeyQueryResult?> GetByCodeAsync(string code);
    Task<SearchWarehousesQueryResult> SearchAsync(SearchWarehousesQuery query);
    Task<List<WarehouseListItem>> GetActiveWarehousesAsync();
    Task<List<WarehouseLookupItem>> GetLookupAsync(bool includeInactive = false);
    Task<WarehouseSummaryItem?> GetSummaryAsync(Guid warehouseBusinessKey);
    Task<WarehouseWithLocationsItem?> GetWithLocationsAsync(Guid warehouseBusinessKey, bool includeInactiveLocations = false);
}
