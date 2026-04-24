namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByBucketKey;

using OysterFx.AppCore.Shared.Queries;

public class GetStockDetailByBucketKeyQuery : IQuery<GetStockDetailByBucketKeyQueryResult>
{
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
}
