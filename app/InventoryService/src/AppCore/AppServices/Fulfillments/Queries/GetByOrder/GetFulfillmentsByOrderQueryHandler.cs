namespace Insurance.InventoryService.AppCore.AppServices.Fulfillments.Queries.GetByOrder;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByOrder;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetFulfillmentsByOrderQueryHandler
    : QueryHandler<GetFulfillmentsByOrderQuery, GetFulfillmentsByOrderQueryResult>
{
    private readonly IFulfillmentQueryRepository _repository;

    public GetFulfillmentsByOrderQueryHandler(IFulfillmentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetFulfillmentsByOrderQueryResult>> ExecuteAsync(GetFulfillmentsByOrderQuery request)
    {
        var items = await _repository.GetByOrderAsync(request.OrderRef);
        return QueryResult<GetFulfillmentsByOrderQueryResult>.Success(new GetFulfillmentsByOrderQueryResult { Items = items });
    }
}
