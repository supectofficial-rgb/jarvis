namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetStockAging;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetStockAging;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetStockAgingQueryHandler : QueryHandler<GetStockAgingQuery, GetStockAgingQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetStockAgingQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetStockAgingQueryResult>> ExecuteAsync(GetStockAgingQuery request)
    {
        var items = await _repository.GetStockAgingAsync(request);
        return QueryResult<GetStockAgingQueryResult>.Success(new GetStockAgingQueryResult
        {
            Items = items
        });
    }
}
