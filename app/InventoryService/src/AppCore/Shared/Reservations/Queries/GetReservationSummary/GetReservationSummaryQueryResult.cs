namespace Insurance.InventoryService.AppCore.Shared.Reservations.Queries.GetReservationSummary;

public class GetReservationSummaryQueryResult
{
    public Guid ReservationBusinessKey { get; set; }
    public decimal RequestedQuantity { get; set; }
    public decimal AllocatedQuantity { get; set; }
    public decimal ConsumedQuantity { get; set; }
    public decimal RemainingQuantity { get; set; }
    public string Status { get; set; } = string.Empty;
}
