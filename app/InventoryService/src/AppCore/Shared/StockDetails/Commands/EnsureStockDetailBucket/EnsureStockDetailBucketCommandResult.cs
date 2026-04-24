namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Commands.EnsureStockDetailBucket;

public class EnsureStockDetailBucketCommandResult
{
    public Guid StockDetailBusinessKey { get; set; }
    public bool Created { get; set; }
    public decimal QuantityOnHand { get; set; }
}
