namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateInventoryDocument;

using OysterFx.AppCore.Shared.Commands;

public class CreateInventoryDocumentCommand : ICommand<CreateInventoryDocumentCommandResult>
{
    public string? DocumentNo { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? ReferenceType { get; set; }
    public Guid? ReferenceBusinessId { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid SellerRef { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
    public string? IdempotencyKey { get; set; }
    public string? ReasonCode { get; set; }
    public List<CreateInventoryDocumentLineItem> Lines { get; set; } = new();
}

public class CreateInventoryDocumentLineItem
{
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
