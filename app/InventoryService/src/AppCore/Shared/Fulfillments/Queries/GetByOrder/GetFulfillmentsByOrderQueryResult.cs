namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByOrder;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.Common;

public class GetFulfillmentsByOrderQueryResult
{
    public List<FulfillmentListItem> Items { get; set; } = new();
}
