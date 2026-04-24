namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetFulfillmentByBusinessKeyQuery : IQuery<GetFulfillmentByBusinessKeyQueryResult>
{
    public GetFulfillmentByBusinessKeyQuery(Guid fulfillmentBusinessKey)
    {
        FulfillmentBusinessKey = fulfillmentBusinessKey;
    }

    public Guid FulfillmentBusinessKey { get; }
}
