namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationSummary;

using OysterFx.AppCore.Shared.Queries;

public class GetReservationSummaryQuery : IQuery<GetReservationSummaryQueryResult>
{
    public GetReservationSummaryQuery(Guid reservationBusinessKey)
    {
        ReservationBusinessKey = reservationBusinessKey;
    }

    public Guid ReservationBusinessKey { get; }
}
