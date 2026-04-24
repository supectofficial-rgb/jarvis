namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetStockAging;

using OysterFx.AppCore.Shared.Queries;

public class GetStockAgingQuery : IQuery<GetStockAgingQueryResult>
{
    public Guid? VariantRef { get; set; }
    public Guid? SellerRef { get; set; }
    public Guid? WarehouseRef { get; set; }
    public Guid? LocationRef { get; set; }
    public Guid? QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public int MinAgeDays { get; set; } = 0;
    public bool IncludeEmptyBuckets { get; set; } = false;
    public DateTime? AsOfUtc { get; set; }
}
