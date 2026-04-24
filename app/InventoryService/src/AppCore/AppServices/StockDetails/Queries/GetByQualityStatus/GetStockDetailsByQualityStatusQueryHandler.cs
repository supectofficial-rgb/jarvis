namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetByQualityStatus;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByQualityStatus;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetStockDetailsByQualityStatusQueryHandler : QueryHandler<GetStockDetailsByQualityStatusQuery, GetStockDetailsByQualityStatusQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetStockDetailsByQualityStatusQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetStockDetailsByQualityStatusQueryResult>> ExecuteAsync(GetStockDetailsByQualityStatusQuery request)
    {
        var items = await _repository.GetByQualityStatusAsync(request.QualityStatusRef);
        return QueryResult<GetStockDetailsByQualityStatusQueryResult>.Success(new GetStockDetailsByQualityStatusQueryResult
        {
            Items = items
        });
    }
}
