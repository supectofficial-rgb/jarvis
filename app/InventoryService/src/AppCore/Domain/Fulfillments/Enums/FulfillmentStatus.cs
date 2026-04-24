namespace Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;

public enum FulfillmentStatus
{
    Pending = 1,
    Picked = 2,
    Packed = 3,
    Shipped = 4,
    Returned = 5,
    PartiallyReturned = 6,
    Cancelled = 7
}
