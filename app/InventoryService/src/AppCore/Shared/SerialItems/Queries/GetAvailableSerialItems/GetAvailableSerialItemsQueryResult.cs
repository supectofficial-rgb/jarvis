namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetAvailableSerialItems;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.Common;

public class GetAvailableSerialItemsQueryResult
{
    public List<SerialItemListItem> Items { get; set; } = new();
}
