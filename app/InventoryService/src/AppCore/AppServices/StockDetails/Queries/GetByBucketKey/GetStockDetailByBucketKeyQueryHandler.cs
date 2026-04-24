namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetByBucketKey;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByBucketKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetStockDetailByBucketKeyQueryHandler : QueryHandler<GetStockDetailByBucketKeyQuery, GetStockDetailByBucketKeyQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetStockDetailByBucketKeyQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetStockDetailByBucketKeyQueryResult>> ExecuteAsync(GetStockDetailByBucketKeyQuery request)
    {
        var item = await _repository.GetByBucketKeyAsync(
            request.VariantRef,
            request.SellerRef,
            request.WarehouseRef,
            request.LocationRef,
            request.QualityStatusRef,
            request.LotBatchNo);

        if (item is null)
            return QueryResult<GetStockDetailByBucketKeyQueryResult>.Fail("Stock detail bucket was not found.", "NOT_FOUND");

        return QueryResult<GetStockDetailByBucketKeyQueryResult>.Success(new GetStockDetailByBucketKeyQueryResult { Item = item });
    }
}
