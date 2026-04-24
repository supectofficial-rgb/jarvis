namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetByVariant;

using Insurance.InventoryService.AppCore.Shared.Reservations.Queries.Common;

public class GetReservationsByVariantQueryResult
{
    public List<ReservationListItem> Items { get; set; } = new();
}
