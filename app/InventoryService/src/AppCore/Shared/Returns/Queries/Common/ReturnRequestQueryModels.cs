namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries.Common;

public class ReturnRequestListItem
{
    public Guid ReturnRequestBusinessKey { get; set; }
    public Guid OrderRef { get; set; }
    public Guid OrderItemRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ReasonCode { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
}
