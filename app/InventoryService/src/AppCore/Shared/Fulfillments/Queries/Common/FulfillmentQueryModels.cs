namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.Common;

public class FulfillmentListItem
{
    public Guid FulfillmentBusinessKey { get; set; }
    public Guid OrderRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid ChannelRef { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PickedAt { get; set; }
    public DateTime? PackedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
}
