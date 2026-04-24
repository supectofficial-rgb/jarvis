namespace Insurance.InventoryService.AppCore.Domain.Fulfillments.Entities;

public enum FulfillmentLineStatus
{
    Pending = 1,
    PartiallyPicked = 2,
    Picked = 3,
    PartiallyPacked = 4,
    Packed = 5,
    PartiallyShipped = 6,
    Shipped = 7,
    PartiallyReturned = 8,
    Returned = 9
}
