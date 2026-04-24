namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetLowStockBuckets;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetLowStockBuckets;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetLowStockBucketsQueryHandler : QueryHandler<GetLowStockBucketsQuery, GetLowStockBucketsQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetLowStockBucketsQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetLowStockBucketsQueryResult>> ExecuteAsync(GetLowStockBucketsQuery request)
    {
        var items = await _repository.GetLowStockBucketsAsync(request);
        return QueryResult<GetLowStockBucketsQueryResult>.Success(new GetLowStockBucketsQueryResult
        {
            Items = items
        });
    }
}
