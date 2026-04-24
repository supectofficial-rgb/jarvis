namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByStatus;

using OysterFx.AppCore.Shared.Queries;

public class GetFulfillmentsByStatusQuery : IQuery<GetFulfillmentsByStatusQueryResult>
{
    public GetFulfillmentsByStatusQuery(string status)
    {
        Status = status;
    }

    public string Status { get; }
}
