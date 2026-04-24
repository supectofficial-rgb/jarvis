namespace Insurance.InventoryService.AppCore.Shared.Warehouse.Warehouses.Queries.GetWarehouseSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetWarehouseSummaryQuery : IQuery<GetWarehouseSummaryQueryResult>
{
    public GetWarehouseSummaryQuery(Guid warehouseBusinessKey)
    {
        WarehouseBusinessKey = warehouseBusinessKey;
    }

    public Guid WarehouseBusinessKey { get; }
}
