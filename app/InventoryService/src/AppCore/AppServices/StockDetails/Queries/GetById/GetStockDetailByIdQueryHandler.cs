namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetStockDetailByIdQueryHandler : QueryHandler<GetStockDetailByIdQuery, GetStockDetailByIdQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetStockDetailByIdQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetStockDetailByIdQueryResult>> ExecuteAsync(GetStockDetailByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.StockDetailId);
        if (item is null)
            return QueryResult<GetStockDetailByIdQueryResult>.Fail("Stock detail was not found.", "NOT_FOUND");

        return QueryResult<GetStockDetailByIdQueryResult>.Success(new GetStockDetailByIdQueryResult { Item = item });
    }
}
