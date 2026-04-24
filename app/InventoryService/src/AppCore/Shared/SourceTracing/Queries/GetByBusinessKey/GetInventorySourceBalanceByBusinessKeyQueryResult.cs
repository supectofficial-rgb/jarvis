namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetByBusinessKey;

public class GetInventorySourceBalanceByBusinessKeyQueryResult
{
    public Guid SourceBalanceBusinessKey { get; set; }
    public string SourceType { get; set; } = string.Empty;
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal AllocatedQty { get; set; }
    public decimal ConsumedQty { get; set; }
    public decimal AvailableQty { get; set; }
    public decimal RemainingQty { get; set; }
    public string Status { get; set; } = string.Empty;
}
