namespace Insurance.InventoryService.AppCore.AppServices.Reservations.Queries.GetReservationSummary;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries;
using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationSummary;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetReservationSummaryQueryHandler
    : QueryHandler<GetReservationSummaryQuery, GetReservationSummaryQueryResult>
{
    private readonly IInventoryReservationQueryRepository _repository;

    public GetReservationSummaryQueryHandler(IInventoryReservationQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetReservationSummaryQueryResult>> ExecuteAsync(GetReservationSummaryQuery request)
    {
        var item = await _repository.GetSummaryAsync(request.ReservationBusinessKey);
        if (item is null)
            return QueryResult<GetReservationSummaryQueryResult>.Fail("Reservation was not found.", "NOT_FOUND");

        return QueryResult<GetReservationSummaryQueryResult>.Success(new GetReservationSummaryQueryResult
        {
            ReservationBusinessKey = item.ReservationBusinessKey,
            RequestedQuantity = item.RequestedQuantity,
            AllocatedQuantity = item.AllocatedQuantity,
            ConsumedQuantity = item.ConsumedQuantity,
            RemainingQuantity = item.RequestedQuantity - item.ConsumedQuantity,
            Status = item.Status
        });
    }
}
