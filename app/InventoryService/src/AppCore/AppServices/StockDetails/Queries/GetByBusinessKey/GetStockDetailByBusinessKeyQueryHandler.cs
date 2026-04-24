namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetStockDetailByBusinessKeyQueryHandler
    : QueryHandler<GetStockDetailByBusinessKeyQuery, GetStockDetailByBusinessKeyQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetStockDetailByBusinessKeyQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetStockDetailByBusinessKeyQueryResult>> ExecuteAsync(GetStockDetailByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.StockDetailBusinessKey);
        if (item is null)
            return QueryResult<GetStockDetailByBusinessKeyQueryResult>.Fail("Stock detail was not found.", "NOT_FOUND");

        return QueryResult<GetStockDetailByBusinessKeyQueryResult>.Success(item);
    }
}

