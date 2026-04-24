namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.Common;

public class ReservationListItem
{
    public Guid ReservationBusinessKey { get; set; }
    public Guid OrderRef { get; set; }
    public Guid OrderItemRef { get; set; }
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid ChannelRef { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal AllocatedQuantity { get; set; }
    public decimal ConsumedQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}

public class ReservationAllocationListItem
{
    public Guid AllocationBusinessKey { get; set; }
    public Guid ReservationRef { get; set; }
    public Guid? StockDetailRef { get; set; }
    public Guid VariantRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid LocationRef { get; set; }
    public Guid QualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public Guid? SerialRef { get; set; }
    public decimal AllocatedQty { get; set; }
    public decimal ReleasedQty { get; set; }
    public decimal ConsumedQty { get; set; }
    public DateTime AllocatedAt { get; set; }
}
