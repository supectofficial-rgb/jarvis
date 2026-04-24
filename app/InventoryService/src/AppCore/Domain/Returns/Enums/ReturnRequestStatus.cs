namespace Insurance.InventoryService.AppCore.Domain.Returns.Entities;

public enum ReturnRequestStatus
{
    Requested = 1,
    Approved = 2,
    Rejected = 3,
    Received = 4,
    Closed = 5
}
