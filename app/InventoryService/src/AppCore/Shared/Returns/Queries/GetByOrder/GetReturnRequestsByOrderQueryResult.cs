namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByOrder;

using Insurance.InventoryService.AppCore.Shared.Returns.Queries.Common;

public class GetReturnRequestsByOrderQueryResult
{
    public List<ReturnRequestListItem> Items { get; set; } = new();
}
