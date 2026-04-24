namespace Insurance.InventoryService.AppCore.Shared.Fulfillments.Queries.GetFulfillmentSummary;

public class GetFulfillmentSummaryQueryResult
{
    public Guid FulfillmentBusinessKey { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? PickedAt { get; set; }
    public DateTime? PackedAt { get; set; }
    public DateTime? ShippedAt { get; set; }
}
