namespace Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByBusinessKey;

public class GetStockDetailByBusinessKeyQueryResult
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
