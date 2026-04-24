namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByStatus;

using Insurance.InventoryService.AppCore.Shared.Returns.Queries.Common;

public class GetReturnRequestsByStatusQueryResult
{
    public List<ReturnRequestListItem> Items { get; set; } = new();
}
