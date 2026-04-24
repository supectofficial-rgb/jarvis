namespace Insurance.InventoryService.AppCore.AppServices.Returns.Queries.GetReturnSummary;

using Insurance.InventoryService.AppCore.Shared.Returns.Queries;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetReturnSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReturnSummaryQueryHandler : QueryHandler<GetReturnSummaryQuery, GetReturnSummaryQueryResult>
{
    private readonly IReturnRequestQueryRepository _repository;

    public GetReturnSummaryQueryHandler(IReturnRequestQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReturnSummaryQueryResult>> ExecuteAsync(GetReturnSummaryQuery request)
    {
        var item = await _repository.GetSummaryAsync(request.ReturnRequestBusinessKey);
        if (item is null)
            return QueryResult<GetReturnSummaryQueryResult>.Fail("Return request was not found.", "NOT_FOUND");

        return QueryResult<GetReturnSummaryQueryResult>.Success(new GetReturnSummaryQueryResult
        {
            ReturnRequestBusinessKey = item.ReturnRequestBusinessKey,
            Status = item.Status,
            ReasonCode = item.ReasonCode,
            RequestedAt = item.RequestedAt,
            ApprovedAt = item.ApprovedAt,
            ReceivedAt = item.ReceivedAt
        });
    }
}
