namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetReturnRequestByBusinessKeyQuery : IQuery<GetReturnRequestByBusinessKeyQueryResult>
{
    public GetReturnRequestByBusinessKeyQuery(Guid returnRequestBusinessKey)
    {
        ReturnRequestBusinessKey = returnRequestBusinessKey;
    }

    public Guid ReturnRequestBusinessKey { get; }
}
