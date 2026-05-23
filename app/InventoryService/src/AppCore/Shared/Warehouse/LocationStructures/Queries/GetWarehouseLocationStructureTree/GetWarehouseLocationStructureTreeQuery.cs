namespace Insurance.InventoryService.AppCore.Shared.Warehouse.LocationStructures.Queries.GetWarehouseLocationStructureTree;

using OysterFx.AppCore.Shared.Queries;

public sealed class GetWarehouseLocationStructureTreeQuery : IQuery<GetWarehouseLocationStructureTreeQueryResult>
{
    public GetWarehouseLocationStructureTreeQuery(Guid warehouseRef, bool includeInactive = false)
    {
        WarehouseRef = warehouseRef;
        IncludeInactive = includeInactive;
    }

    public Guid WarehouseRef { get; }
    public bool IncludeInactive { get; }
}
