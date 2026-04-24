namespace Insurance.InventoryService.AppCore.AppServices.Fulfillments.Queries.GetFulfillmentSummary;

using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries;
using Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetFulfillmentSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetFulfillmentSummaryQueryHandler
    : QueryHandler<GetFulfillmentSummaryQuery, GetFulfillmentSummaryQueryResult>
{
    private readonly IFulfillmentQueryRepository _repository;

    public GetFulfillmentSummaryQueryHandler(IFulfillmentQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetFulfillmentSummaryQueryResult>> ExecuteAsync(GetFulfillmentSummaryQuery request)
    {
        var item = await _repository.GetSummaryAsync(request.FulfillmentBusinessKey);
        if (item is null)
            return QueryResult<GetFulfillmentSummaryQueryResult>.Fail("Fulfillment was not found.", "NOT_FOUND");

        return QueryResult<GetFulfillmentSummaryQueryResult>.Success(new GetFulfillmentSummaryQueryResult
        {
            FulfillmentBusinessKey = item.FulfillmentBusinessKey,
            Status = item.Status,
            PickedAt = item.PickedAt,
            PackedAt = item.PackedAt,
            ShippedAt = item.ShippedAt
        });
    }
}
