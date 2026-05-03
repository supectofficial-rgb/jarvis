namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Queries.GetByBusinessKey;

public class GetInventoryDocumentByBusinessKeyQueryResult
{
    public Guid DocumentBusinessKey { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public Guid? ReferenceBusinessId { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid SellerRef { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PostedAt { get; set; }
    public Guid? PostedTransactionRef { get; set; }
    public string? ReasonCode { get; set; }
    public List<InventoryDocumentLineQueryItem> Lines { get; set; } = new();
}

public class InventoryDocumentLineQueryItem
{
    public Guid LineBusinessKey { get; set; }
    public Guid VariantRef { get; set; }
    public decimal Qty { get; set; }
    public Guid UomRef { get; set; }
    public decimal BaseQty { get; set; }
    public Guid BaseUomRef { get; set; }
    public Guid? SourceLocationRef { get; set; }
    public Guid? DestinationLocationRef { get; set; }
    public Guid? QualityStatusRef { get; set; }
    public Guid? FromQualityStatusRef { get; set; }
    public Guid? ToQualityStatusRef { get; set; }
    public string? LotBatchNo { get; set; }
    public string? ReasonCode { get; set; }
    public string? AdjustmentDirection { get; set; }
}
