namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByWarehouse;

using OysterFx.AppCore.Shared.Queries;

public class GetStockDetailsByWarehouseQuery : IQuery<GetStockDetailsByWarehouseQueryResult>
{
    public GetStockDetailsByWarehouseQuery(Guid warehouseRef)
    {
        WarehouseRef = warehouseRef;
    }

    public Guid WarehouseRef { get; }
}
