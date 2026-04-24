namespace Insurance.InventoryService.AppCore.AppServices.Returns.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Returns.Queries;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReturnRequestByBusinessKeyQueryHandler
    : QueryHandler<GetReturnRequestByBusinessKeyQuery, GetReturnRequestByBusinessKeyQueryResult>
{
    private readonly IReturnRequestQueryRepository _repository;

    public GetReturnRequestByBusinessKeyQueryHandler(IReturnRequestQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReturnRequestByBusinessKeyQueryResult>> ExecuteAsync(GetReturnRequestByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.ReturnRequestBusinessKey);
        if (item is null)
            return QueryResult<GetReturnRequestByBusinessKeyQueryResult>.Fail("Return request was not found.", "NOT_FOUND");

        return QueryResult<GetReturnRequestByBusinessKeyQueryResult>.Success(item);
    }
}
