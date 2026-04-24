namespace Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetAllocationsByReservationId;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.Common;

public class GetInventorySourceAllocationsByReservationIdQueryResult
{
    public List<InventorySourceAllocationListItem> Items { get; set; } = new();
}
