namespace Insurance.InventoryService.AppCore.Domain.StockDetails.Entities;

public enum SerialItemStatus
{
    Available = 1,
    Reserved = 2,
    Issued = 3,
    Returned = 4,
    Scrapped = 5,
    Quarantine = 6
}
