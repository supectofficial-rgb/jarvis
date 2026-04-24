namespace Insurance.InventoryService.AppCore.AppServices.Fulfillments.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetFulfillmentByIdQueryHandler : QueryHandler<GetFulfillmentByIdQuery, GetFulfillmentByIdQueryResult>
{
    private readonly IFulfillmentQueryRepository _repository;

    public GetFulfillmentByIdQueryHandler(IFulfillmentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetFulfillmentByIdQueryResult>> ExecuteAsync(GetFulfillmentByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.FulfillmentId);
        if (item is null)
            return QueryResult<GetFulfillmentByIdQueryResult>.Fail("Fulfillment was not found.", "NOT_FOUND");

        return QueryResult<GetFulfillmentByIdQueryResult>.Success(new GetFulfillmentByIdQueryResult { Item = item });
    }
}
