namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries;

using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetLocationStructureValues;
using Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetWarehouseLocationStructureTree;
using OysterFx.AppCore.Shared.Queries;

public interface ILocationStructureQueryRepository : IQueryRepository
{
    Task<GetWarehouseLocationStructureTreeQueryResult?> GetTreeAsync(Guid warehouseRef, bool includeInactive = false);
    Task<GetLocationStructureValuesQueryResult?> GetValuesAsync(Guid structureRef, bool includeInactive = false);
}
