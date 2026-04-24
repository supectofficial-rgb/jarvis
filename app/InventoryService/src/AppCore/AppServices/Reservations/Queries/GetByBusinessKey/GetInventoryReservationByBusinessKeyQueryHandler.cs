namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Queries.GetByBusinessKey;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByBusinessKey;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetInventoryReservationByBusinessKeyQueryHandler
    : QueryHandler<GetInventoryReservationByBusinessKeyQuery, GetInventoryReservationByBusinessKeyQueryResult>
{
    private readonly IInventoryReservationQueryRepository _repository;

    public GetInventoryReservationByBusinessKeyQueryHandler(IInventoryReservationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetInventoryReservationByBusinessKeyQueryResult>> ExecuteAsync(GetInventoryReservationByBusinessKeyQuery request)
    {
        var item = await _repository.GetByBusinessKeyAsync(request.ReservationBusinessKey);
        if (item is null)
            return QueryResult<GetInventoryReservationByBusinessKeyQueryResult>.Fail("Reservation was not found.", "NOT_FOUND");

        return QueryResult<GetInventoryReservationByBusinessKeyQueryResult>.Success(item);
    }
}
