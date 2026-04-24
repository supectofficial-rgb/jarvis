namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetByVariant;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByVariant;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetStockDetailsByVariantQueryHandler : QueryHandler<GetStockDetailsByVariantQuery, GetStockDetailsByVariantQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetStockDetailsByVariantQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetStockDetailsByVariantQueryResult>> ExecuteAsync(GetStockDetailsByVariantQuery request)
    {
        var items = await _repository.GetByVariantAsync(request.VariantRef);
        return QueryResult<GetStockDetailsByVariantQueryResult>.Success(new GetStockDetailsByVariantQueryResult { Items = items });
    }
}
