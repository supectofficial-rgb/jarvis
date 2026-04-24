namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Queries.GetReservationAllocationsByStockDetailId;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationAllocationsByStockDetailId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReservationAllocationsByStockDetailIdQueryHandler
    : QueryHandler<GetReservationAllocationsByStockDetailIdQuery, GetReservationAllocationsByStockDetailIdQueryResult>
{
    private readonly IInventoryReservationQueryRepository _repository;

    public GetReservationAllocationsByStockDetailIdQueryHandler(IInventoryReservationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReservationAllocationsByStockDetailIdQueryResult>> ExecuteAsync(GetReservationAllocationsByStockDetailIdQuery request)
    {
        var items = await _repository.GetAllocationsByStockDetailIdAsync(request.StockDetailBusinessKey);
        return QueryResult<GetReservationAllocationsByStockDetailIdQueryResult>.Success(
            new GetReservationAllocationsByStockDetailIdQueryResult { Items = items });
    }
}
