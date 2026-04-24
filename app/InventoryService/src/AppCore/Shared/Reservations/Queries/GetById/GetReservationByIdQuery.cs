namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetById;

using OysterFx.AppCore.Shared.Queries;

public class GetReservationByIdQuery : IQuery<GetReservationByIdQueryResult>
{
    public GetReservationByIdQuery(Guid reservationId)
    {
        ReservationId = reservationId;
    }

    public Guid ReservationId { get; }
}
