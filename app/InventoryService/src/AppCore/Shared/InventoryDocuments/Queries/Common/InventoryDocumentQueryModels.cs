namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.Common;

public class InventoryDocumentListItem
{
    public Guid DocumentBusinessKey { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ExternalReferenceNo { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceBusinessId { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid SellerRef { get; set; }
    public string? ReceivedBy { get; set; }
    public string? DeliveredBy { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PostedAt { get; set; }
    public Guid? PostedTransactionRef { get; set; }
    public string? ReasonCode { get; set; }
}
