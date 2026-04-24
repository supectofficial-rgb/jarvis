namespace Insurance.InventoryService.AppCore.Shared.Returns.Queries.GetReturnSummary;

public class GetReturnSummaryQueryResult
{
    public Guid ReturnRequestBusinessKey { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? ReasonCode { get; set; }
    public DateTime RequestedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? ReceivedAt { get; set; }
}
