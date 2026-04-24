namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.Common;

public class InventorySourceBalanceListItem
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

public class InventorySourceBalanceSummaryItem
{
    public Guid SourceBalanceBusinessKey { get; set; }
    public decimal ReceivedQty { get; set; }
    public decimal AllocatedQty { get; set; }
    public decimal ConsumedQty { get; set; }
    public decimal AvailableQty { get; set; }
    public decimal RemainingQty { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class InventorySourceAllocationListItem
{
    public Guid AllocationBusinessKey { get; set; }
    public Guid SourceBalanceRef { get; set; }
    public Guid ReservationRef { get; set; }
    public Guid? ReservationAllocationRef { get; set; }
    public Guid VariantRef { get; set; }
    public decimal AllocatedQty { get; set; }
    public decimal ReleasedQty { get; set; }
    public decimal ConsumedQty { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class InventorySourceConsumptionListItem
{
    public Guid ConsumptionBusinessKey { get; set; }
    public Guid OutboundTransactionRef { get; set; }
    public Guid? OutboundTransactionLineRef { get; set; }
    public Guid SourceBalanceRef { get; set; }
    public Guid VariantRef { get; set; }
    public decimal ConsumedQty { get; set; }
    public string? ReasonCode { get; set; }
    public DateTime CreatedAt { get; set; }
}
