namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.EnsureStockDetailBucket;

using OysterFx.AppCore.Shared.Commands;

public class EnsureStockDetailBucketCommand : ICommand<EnsureStockDetailBucketCommandResult>
{
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public decimal OpeningQuantityOnCreate { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
}
