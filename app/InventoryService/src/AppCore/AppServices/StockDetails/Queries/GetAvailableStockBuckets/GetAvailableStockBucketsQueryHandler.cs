namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetAvailableStockBuckets;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetAvailableStockBuckets;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetAvailableStockBucketsQueryHandler : QueryHandler<GetAvailableStockBucketsQuery, GetAvailableStockBucketsQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetAvailableStockBucketsQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetAvailableStockBucketsQueryResult>> ExecuteAsync(GetAvailableStockBucketsQuery request)
    {
        var items = await _repository.GetAvailableBucketsAsync(request);
        return QueryResult<GetAvailableStockBucketsQueryResult>.Success(new GetAvailableStockBucketsQueryResult
        {
            Items = items
        });
    }
}
