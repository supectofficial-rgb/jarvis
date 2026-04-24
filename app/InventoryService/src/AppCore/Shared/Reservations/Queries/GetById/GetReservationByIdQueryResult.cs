namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetById;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.Common;

public class GetReservationByIdQueryResult
{
    public ReservationListItem Item { get; set; } = new();
}
