namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetFulfillmentSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetFulfillmentSummaryQuery : IQuery<GetFulfillmentSummaryQueryResult>
{
    public GetFulfillmentSummaryQuery(Guid fulfillmentBusinessKey)
    {
        FulfillmentBusinessKey = fulfillmentBusinessKey;
    }

    public Guid FulfillmentBusinessKey { get; }
}
