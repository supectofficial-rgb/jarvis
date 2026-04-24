namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByBusinessKey;

public class GetInventoryReservationByBusinessKeyQueryResult
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
