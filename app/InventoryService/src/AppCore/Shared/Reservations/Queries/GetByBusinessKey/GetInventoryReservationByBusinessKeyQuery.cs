namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByBusinessKey;

using OysterFx.AppCore.Shared.Queries;

public class GetInventoryReservationByBusinessKeyQuery : IQuery<GetInventoryReservationByBusinessKeyQueryResult>
{
    public GetInventoryReservationByBusinessKeyQuery(Guid reservationBusinessKey)
    {
        ReservationBusinessKey = reservationBusinessKey;
    }

    public Guid ReservationBusinessKey { get; }
}
