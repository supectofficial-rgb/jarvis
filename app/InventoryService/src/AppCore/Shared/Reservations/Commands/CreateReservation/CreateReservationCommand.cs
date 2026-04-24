namespace Insurance.InventoryService.AppCore.Shared.Reservations.Commands.CreateReservation;

using OysterFx.AppCore.Shared.Commands;

public class CreateReservationCommand : ICommand<Guid>
{
    public Guid OrderRef { get; set; }
    public Guid OrderItemRef { get; set; }
    public Guid VariantRef { get; set; }
    public Guid SellerRef { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid ChannelRef { get; set; }
    public decimal RequestedQuantity { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? CorrelationId { get; set; }
    public string? IdempotencyKey { get; set; }
    public string? ReasonCode { get; set; }
}
