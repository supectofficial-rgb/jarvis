namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.Common;

public class StockDetailListItem
{
    public Guid StockDetailBusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public decimal QuantityOnHand { get; set; }
    public DateTime FirstReceivedAt { get; set; }
    public DateTime LastReceivedAt { get; set; }
    public DateTime? LastIssuedAt { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}

public class VariantStockSummaryItem
{
    public Guid VariantRef { get; set; }
    public decimal TotalQuantityOnHand { get; set; }
    public int BucketCount { get; set; }
}

public class WarehouseStockSummaryItem
{
    public Guid WarehouseRef { get; set; }
    public decimal TotalQuantityOnHand { get; set; }
    public int BucketCount { get; set; }
    public int VariantCount { get; set; }
}

public class SellerStockSummaryItem
{
    public Guid SellerRef { get; set; }
    public decimal TotalQuantityOnHand { get; set; }
    public int BucketCount { get; set; }
    public int VariantCount { get; set; }
}

public class StockAgingItem
{
    public Guid StockDetailBusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public decimal QuantityOnHand { get; set; }
    public DateTime FirstReceivedAt { get; set; }
    public DateTime LastReceivedAt { get; set; }
    public DateTime? LastIssuedAt { get; set; }
    public DateTime AsOfUtc { get; set; }
    public int AgeDays { get; set; }
}
