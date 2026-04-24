namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.Common;

public class GetFulfillmentByIdQueryResult
{
    public FulfillmentListItem Item { get; set; } = new();
}
