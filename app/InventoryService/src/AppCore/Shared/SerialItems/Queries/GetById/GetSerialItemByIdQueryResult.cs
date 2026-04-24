namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.Common;

public class GetSerialItemByIdQueryResult
{
    public SerialItemListItem Item { get; set; } = new();
}
