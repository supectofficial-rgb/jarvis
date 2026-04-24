namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetEmptyStockBuckets;

using OysterFx.AppCore.Shared.Queries;

public class GetEmptyStockBucketsQuery : IQuery<GetEmptyStockBucketsQueryResult>
{
    public Guid? VariantRef { get; set; }
    public Guid? SellerRef { get; set; }
    public Guid? WarehouseRef { get; set; }
    public Guid? LocationRef { get; set; }
    public Guid? QualityStatusRef { get; set; }
}
