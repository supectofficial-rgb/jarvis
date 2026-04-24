namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Fulfillments.Entities;

using Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;

public class FulfillmentReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid OrderRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid ChannelRef { get; set; }
    public FulfillmentStatus Status { get; set; }
    public DateTime? PickedAt { get; set; }
    public DateTime? PackedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
}
