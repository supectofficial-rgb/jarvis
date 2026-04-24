namespace Insurance.InventoryService.AppCore.AppServices.Fulfillments.Queries.GetByStatus;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByStatus;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetFulfillmentsByStatusQueryHandler
    : QueryHandler<GetFulfillmentsByStatusQuery, GetFulfillmentsByStatusQueryResult>
{
    private readonly IFulfillmentQueryRepository _repository;

    public GetFulfillmentsByStatusQueryHandler(IFulfillmentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetFulfillmentsByStatusQueryResult>> ExecuteAsync(GetFulfillmentsByStatusQuery request)
    {
        var items = await _repository.GetByStatusAsync(request.Status);
        return QueryResult<GetFulfillmentsByStatusQueryResult>.Success(new GetFulfillmentsByStatusQueryResult { Items = items });
    }
}
