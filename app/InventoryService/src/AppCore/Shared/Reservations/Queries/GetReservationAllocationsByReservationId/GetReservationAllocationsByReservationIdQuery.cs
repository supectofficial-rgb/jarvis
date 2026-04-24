namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationAllocationsByReservationId;

using OysterFx.AppCore.Shared.Queries;

public class GetReservationAllocationsByReservationIdQuery : IQuery<GetReservationAllocationsByReservationIdQueryResult>
{
    public GetReservationAllocationsByReservationIdQuery(Guid reservationBusinessKey)
    {
        ReservationBusinessKey = reservationBusinessKey;
    }

    public Guid ReservationBusinessKey { get; }
}
