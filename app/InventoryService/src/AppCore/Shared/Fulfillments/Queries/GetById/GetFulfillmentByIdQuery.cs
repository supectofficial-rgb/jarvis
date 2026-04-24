namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetFulfillmentByIdQuery : IQuery<GetFulfillmentByIdQueryResult>
{
    public GetFulfillmentByIdQuery(Guid fulfillmentId)
    {
        FulfillmentId = fulfillmentId;
    }

    public Guid FulfillmentId { get; }
}
