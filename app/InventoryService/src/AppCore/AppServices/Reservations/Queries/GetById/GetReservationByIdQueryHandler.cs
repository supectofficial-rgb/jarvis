namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetById;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReservationByIdQueryHandler : QueryHandler<GetReservationByIdQuery, GetReservationByIdQueryResult>
{
    private readonly IInventoryReservationQueryRepository _repository;

    public GetReservationByIdQueryHandler(IInventoryReservationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReservationByIdQueryResult>> ExecuteAsync(GetReservationByIdQuery request)
    {
        var item = await _repository.GetByIdAsync(request.ReservationId);
        if (item is null)
            return QueryResult<GetReservationByIdQueryResult>.Fail("Reservation was not found.", "NOT_FOUND");

        return QueryResult<GetReservationByIdQueryResult>.Success(new GetReservationByIdQueryResult { Item = item });
    }
}
