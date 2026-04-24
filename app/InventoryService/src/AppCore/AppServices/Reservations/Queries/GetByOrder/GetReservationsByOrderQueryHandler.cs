namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Queries.GetByOrder;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByOrder;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReservationsByOrderQueryHandler
    : QueryHandler<GetReservationsByOrderQuery, GetReservationsByOrderQueryResult>
{
    private readonly IInventoryReservationQueryRepository _repository;

    public GetReservationsByOrderQueryHandler(IInventoryReservationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReservationsByOrderQueryResult>> ExecuteAsync(GetReservationsByOrderQuery request)
    {
        var items = await _repository.GetByOrderAsync(request.OrderRef);
        return QueryResult<GetReservationsByOrderQueryResult>.Success(new GetReservationsByOrderQueryResult { Items = items });
    }
}
