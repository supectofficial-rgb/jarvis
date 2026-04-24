namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetActiveReservations;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.Common;

public class GetActiveReservationsQueryResult
{
    public List<ReservationListItem> Items { get; set; } = new();
}
