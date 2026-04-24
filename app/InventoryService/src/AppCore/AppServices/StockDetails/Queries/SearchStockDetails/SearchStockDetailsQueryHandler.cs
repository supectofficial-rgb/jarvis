namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.SearchStockDetails;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.SearchStockDetails;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class SearchStockDetailsQueryHandler : QueryHandler<SearchStockDetailsQuery, SearchStockDetailsQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public SearchStockDetailsQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<SearchStockDetailsQueryResult>> ExecuteAsync(SearchStockDetailsQuery request)
    {
        var result = await _repository.SearchAsync(request);
        return QueryResult<SearchStockDetailsQueryResult>.Success(result);
    }
}
