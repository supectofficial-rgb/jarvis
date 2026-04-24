namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByLotBatch;

using OysterFx.AppCore.Shared.Queries;

public class GetStockDetailsByLotBatchQuery : IQuery<GetStockDetailsByLotBatchQueryResult>
{
    public GetStockDetailsByLotBatchQuery(string lotBatchNo)
    {
        LotBatchNo = lotBatchNo;
    }

    public string LotBatchNo { get; }
}
