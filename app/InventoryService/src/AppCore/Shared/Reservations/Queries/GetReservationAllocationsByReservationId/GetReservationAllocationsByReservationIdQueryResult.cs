namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationAllocationsByReservationId;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.Common;

public class GetReservationAllocationsByReservationIdQueryResult
{
    public List<ReservationAllocationListItem> Items { get; set; } = new();
}
