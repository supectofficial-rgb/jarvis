namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetEmptyStockBuckets;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetEmptyStockBuckets;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetEmptyStockBucketsQueryHandler : QueryHandler<GetEmptyStockBucketsQuery, GetEmptyStockBucketsQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetEmptyStockBucketsQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetEmptyStockBucketsQueryResult>> ExecuteAsync(GetEmptyStockBucketsQuery request)
    {
        var items = await _repository.GetEmptyBucketsAsync(
            request.VariantRef,
            request.SellerRef,
            request.WarehouseRef,
            request.LocationRef,
            request.QualityStatusRef);

        return QueryResult<GetEmptyStockBucketsQueryResult>.Success(new GetEmptyStockBucketsQueryResult
        {
            Items = items
        });
    }
}
