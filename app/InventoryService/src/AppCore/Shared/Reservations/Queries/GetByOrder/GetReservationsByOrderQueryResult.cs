namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByOrder;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.Common;

public class GetReservationsByOrderQueryResult
{
    public List<ReservationListItem> Items { get; set; } = new();
}
