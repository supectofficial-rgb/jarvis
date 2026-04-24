namespace Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.SearchSerialItems;

using Insurance.InventoryService.AppCore.Shared.SerialItems.Queries.Common;

public class SearchSerialItemsQueryResult
{
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<SerialItemListItem> Items { get; set; } = new();
}
