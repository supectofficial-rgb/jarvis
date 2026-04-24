namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.Reservations.Entities;

using Insurance.InventoryService.AppCore.Domain.Reservations.Entities;

public class InventoryReservationReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public Guid OrderRef { get; set; }
    public Guid OrderItemRef { get; set; }
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid ChannelRef { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal AllocatedQuantity { get; set; }
    public decimal ConsumedQuantity { get; set; }
    public InventoryReservationStatus Status { get; set; }
    public DateTime? ExpiresAt { get; set; }
}
