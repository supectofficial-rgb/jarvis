namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetAllocationsByReservationId;

using OysterFx.AppCore.Shared.Queries;

public class GetInventorySourceAllocationsByReservationIdQuery : IQuery<GetInventorySourceAllocationsByReservationIdQueryResult>
{
    public GetInventorySourceAllocationsByReservationIdQuery(Guid reservationRef)
    {
        ReservationRef = reservationRef;
    }

    public Guid ReservationRef { get; }
}
