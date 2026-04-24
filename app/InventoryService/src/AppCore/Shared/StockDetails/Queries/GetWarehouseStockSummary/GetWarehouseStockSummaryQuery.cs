namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetWarehouseStockSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetWarehouseStockSummaryQuery : IQuery<GetWarehouseStockSummaryQueryResult>
{
    public GetWarehouseStockSummaryQuery(Guid warehouseRef)
    {
        WarehouseRef = warehouseRef;
    }

    public Guid WarehouseRef { get; }
}
