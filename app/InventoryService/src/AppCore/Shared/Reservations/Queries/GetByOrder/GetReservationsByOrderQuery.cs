namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByOrder;

using OysterFx.AppCore.Shared.Queries;

public class GetReservationsByOrderQuery : IQuery<GetReservationsByOrderQueryResult>
{
    public GetReservationsByOrderQuery(Guid orderRef)
    {
        OrderRef = orderRef;
    }

    public Guid OrderRef { get; }
}
