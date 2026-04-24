namespace Insurance.InventoryService.AppCore.AppServices.StockDetails.Queries.GetByLocation;

using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries;
using Insurance.InventoryService.AppCore.Shared.StockDetails.Queries.GetByLocation;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetStockDetailsByLocationQueryHandler : QueryHandler<GetStockDetailsByLocationQuery, GetStockDetailsByLocationQueryResult>
{
    private readonly IStockDetailQueryRepository _repository;

    public GetStockDetailsByLocationQueryHandler(IStockDetailQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetStockDetailsByLocationQueryResult>> ExecuteAsync(GetStockDetailsByLocationQuery request)
    {
        var items = await _repository.GetByLocationAsync(request.LocationRef);
        return QueryResult<GetStockDetailsByLocationQueryResult>.Success(new GetStockDetailsByLocationQueryResult { Items = items });
    }
}
