namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Queries.GetReservationAllocationsByReservationId;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationAllocationsByReservationId;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReservationAllocationsByReservationIdQueryHandler
    : QueryHandler<GetReservationAllocationsByReservationIdQuery, GetReservationAllocationsByReservationIdQueryResult>
{
    private readonly IInventoryReservationQueryRepository _repository;

    public GetReservationAllocationsByReservationIdQueryHandler(IInventoryReservationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReservationAllocationsByReservationIdQueryResult>> ExecuteAsync(GetReservationAllocationsByReservationIdQuery request)
    {
        var items = await _repository.GetAllocationsByReservationIdAsync(request.ReservationBusinessKey);
        return QueryResult<GetReservationAllocationsByReservationIdQueryResult>.Success(
            new GetReservationAllocationsByReservationIdQueryResult { Items = items });
    }
}
