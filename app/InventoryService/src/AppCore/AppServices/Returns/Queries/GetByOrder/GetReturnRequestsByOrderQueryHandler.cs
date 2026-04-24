namespace Insurance.InventoryService.AppCore.AppServices.Returns.Queries.GetByOrder;

using Insurance.InventoryService.AppCore.Shared.Returns.Queries;
using Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetByOrder;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReturnRequestsByOrderQueryHandler
    : QueryHandler<GetReturnRequestsByOrderQuery, GetReturnRequestsByOrderQueryResult>
{
    private readonly IReturnRequestQueryRepository _repository;

    public GetReturnRequestsByOrderQueryHandler(IReturnRequestQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReturnRequestsByOrderQueryResult>> ExecuteAsync(GetReturnRequestsByOrderQuery request)
    {
        var items = await _repository.GetByOrderAsync(request.OrderRef);
        return QueryResult<GetReturnRequestsByOrderQueryResult>.Success(new GetReturnRequestsByOrderQueryResult { Items = items });
    }
}
