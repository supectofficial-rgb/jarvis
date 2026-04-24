namespace Insurance.InventoryService.AppCore.AppServices.Returns.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Returns.Queries;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReturnRequestByIdQueryHandler
    : QueryHandler<GetReturnRequestByIdQuery, GetReturnRequestByIdQueryResult>
{
    private readonly IReturnRequestQueryRepository _repository;

    public GetReturnRequestByIdQueryHandler(IReturnRequestQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReturnRequestByIdQueryResult>> ExecuteAsync(GetReturnRequestByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.ReturnRequestId);
        if (item is null)
            return QueryResult<GetReturnRequestByIdQueryResult>.Fail("Return request was not found.", "NOT_FOUND");

        return QueryResult<GetReturnRequestByIdQueryResult>.Success(new GetReturnRequestByIdQueryResult { Item = item });
    }
}
