namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetBySerialNo;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.Common;

public class GetSerialItemBySerialNoQueryResult
{
    public SerialItemListItem Item { get; set; } = new();
}
