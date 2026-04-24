namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.GetByVariant;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.Common;

public class GetSerialItemsByVariantQueryResult
{
    public List<SerialItemListItem> Items { get; set; } = new();
}
