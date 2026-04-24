namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Queries.GetActiveReservations;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetActiveReservations;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetActiveReservationsQueryHandler
    : QueryHandler<GetActiveReservationsQuery, GetActiveReservationsQueryResult>
{
    private readonly IInventoryReservationQueryRepository _repository;

    public GetActiveReservationsQueryHandler(IInventoryReservationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetActiveReservationsQueryResult>> ExecuteAsync(GetActiveReservationsQuery request)
    {
        var items = await _repository.GetActiveAsync();
        return QueryResult<GetActiveReservationsQueryResult>.Success(new GetActiveReservationsQueryResult { Items = items });
    }
}
