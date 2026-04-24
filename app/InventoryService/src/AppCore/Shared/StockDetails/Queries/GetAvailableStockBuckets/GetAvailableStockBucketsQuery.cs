namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetAvailableStockBuckets;

using OysterFx.AppCore.Shared.Queries;

public class GetAvailableStockBucketsQuery : IQuery<GetAvailableStockBucketsQueryResult>
{
    public Guid? VariantRef { get; set; }
    public Guid? SellerRef { get; set; }
    public Guid? WarehouseRef { get; set; }
    public Guid? LocationRef { get; set; }
    public Guid? QualityStatusRef { get; set; }
    public decimal? MinQuantity { get; set; }
}
