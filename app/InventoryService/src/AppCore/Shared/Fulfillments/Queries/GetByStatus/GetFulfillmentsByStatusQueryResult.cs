namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByStatus;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.Common;

public class GetFulfillmentsByStatusQueryResult
{
    public List<FulfillmentListItem> Items { get; set; } = new();
}
