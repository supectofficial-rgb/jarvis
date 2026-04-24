namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByStatus;

using OysterFx.AppCore.Shared.Queries;

public class GetReturnRequestsByStatusQuery : IQuery<GetReturnRequestsByStatusQueryResult>
{
    public GetReturnRequestsByStatusQuery(string status)
    {
        Status = status;
    }

    public string Status { get; }
}
