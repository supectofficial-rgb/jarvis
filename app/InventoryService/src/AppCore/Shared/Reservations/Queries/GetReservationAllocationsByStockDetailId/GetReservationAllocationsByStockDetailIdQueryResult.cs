namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationAllocationsByStockDetailId;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.Common;

public class GetReservationAllocationsByStockDetailIdQueryResult
{
    public List<ReservationAllocationListItem> Items { get; set; } = new();
}
