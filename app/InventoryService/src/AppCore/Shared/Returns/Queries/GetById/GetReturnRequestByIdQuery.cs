namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetReturnRequestByIdQuery : IQuery<GetReturnRequestByIdQueryResult>
{
    public GetReturnRequestByIdQuery(Guid returnRequestId)
    {
        ReturnRequestId = returnRequestId;
    }

    public Guid ReturnRequestId { get; }
}
