namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetLowStockBuckets;

using OysterFx.AppCore.Shared.Queries;

public class GetLowStockBucketsQuery : IQuery<GetLowStockBucketsQueryResult>
{
    public decimal ThresholdQuantity { get; set; }
    public Guid? VariantRef { get; set; }
    public Guid? SellerRef { get; set; }
    public Guid? WarehouseRef { get; set; }
    public Guid? LocationRef { get; set; }
    public Guid? QualityStatusRef { get; set; }
}
