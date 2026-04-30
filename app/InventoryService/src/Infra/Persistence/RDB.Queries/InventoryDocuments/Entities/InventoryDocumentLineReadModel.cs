namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Entities;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;

public class InventoryDocumentLineReadModel
{
    public long Id { get; set; }
    public long InventoryDocumentId { get; set; }
    public Guid BusinessKey { get; set; }
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
    public InventoryAdjustmentDirection? AdjustmentDirection { get; set; }
}
