namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetByLotBatch;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByLotBatch;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetStockDetailsByLotBatchQueryHandler : QueryHandler<GetStockDetailsByLotBatchQuery, GetStockDetailsByLotBatchQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetStockDetailsByLotBatchQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetStockDetailsByLotBatchQueryResult>> ExecuteAsync(GetStockDetailsByLotBatchQuery request)
    {
        var items = await _repository.GetByLotBatchAsync(request.LotBatchNo);
        return QueryResult<GetStockDetailsByLotBatchQueryResult>.Success(new GetStockDetailsByLotBatchQueryResult
        {
            Items = items
        });
    }
}
