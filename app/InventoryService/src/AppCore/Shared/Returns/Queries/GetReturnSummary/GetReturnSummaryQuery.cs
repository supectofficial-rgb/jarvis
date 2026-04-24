namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetReturnSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetReturnSummaryQuery : IQuery<GetReturnSummaryQueryResult>
{
    public GetReturnSummaryQuery(Guid returnRequestBusinessKey)
    {
        ReturnRequestBusinessKey = returnRequestBusinessKey;
    }

    public Guid ReturnRequestBusinessKey { get; }
}
