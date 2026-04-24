namespace Insurance.InventoryService.AppCore.Domain.Reservations.Entities;

public enum InventoryReservationStatus
{
    Pending = 1,
    Confirmed = 2,
    Consumed = 3,
    Released = 4,
    Expired = 5,
    Rejected = 6
}
