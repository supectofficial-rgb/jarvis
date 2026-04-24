namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByOrder;

using OysterFx.AppCore.Shared.Queries;

public class GetFulfillmentsByOrderQuery : IQuery<GetFulfillmentsByOrderQueryResult>
{
    public GetFulfillmentsByOrderQuery(Guid orderRef)
    {
        OrderRef = orderRef;
    }

    public Guid OrderRef { get; }
}
