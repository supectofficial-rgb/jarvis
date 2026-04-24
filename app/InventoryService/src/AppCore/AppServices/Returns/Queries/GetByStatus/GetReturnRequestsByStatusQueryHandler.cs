namespace Insurance.InventoryService.AppCore.AppServices.Returns.Queries.GetByStatus;

using Insurance.InventoryService.AppCore.Shared.Returns.Queries;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByStatus;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReturnRequestsByStatusQueryHandler
    : QueryHandler<GetReturnRequestsByStatusQuery, GetReturnRequestsByStatusQueryResult>
{
    private readonly IReturnRequestQueryRepository _repository;

    public GetReturnRequestsByStatusQueryHandler(IReturnRequestQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReturnRequestsByStatusQueryResult>> ExecuteAsync(GetReturnRequestsByStatusQuery request)
    {
        var items = await _repository.GetByStatusAsync(request.Status);
        return QueryResult<GetReturnRequestsByStatusQueryResult>.Success(new GetReturnRequestsByStatusQueryResult { Items = items });
    }
}
