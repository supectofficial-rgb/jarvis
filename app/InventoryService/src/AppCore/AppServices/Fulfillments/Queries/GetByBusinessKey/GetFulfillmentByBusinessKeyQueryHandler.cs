namespace Insurance.InventoryService.AppCore.AppServices.Fulfillments.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetFulfillmentByBusinessKeyQueryHandler
    : QueryHandler<GetFulfillmentByBusinessKeyQuery, GetFulfillmentByBusinessKeyQueryResult>
{
    private readonly IFulfillmentQueryRepository _repository;

    public GetFulfillmentByBusinessKeyQueryHandler(IFulfillmentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetFulfillmentByBusinessKeyQueryResult>> ExecuteAsync(GetFulfillmentByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.FulfillmentBusinessKey);
        if (item is null)
            return QueryResult<GetFulfillmentByBusinessKeyQueryResult>.Fail("Fulfillment was not found.", "NOT_FOUND");

        return QueryResult<GetFulfillmentByBusinessKeyQueryResult>.Success(item);
    }
}
