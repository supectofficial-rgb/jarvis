namespace Insurance.InventoryService.Infra.Persistence.RDB.Queries.InventoryDocuments.Entities;

using Insurance.InventoryService.AppCore.Domain.InventoryDocuments.Entities;

public class InventoryDocumentReadModel
{
    public long Id { get; set; }
    public Guid BusinessKey { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public InventoryDocumentType DocumentType { get; set; }
    public InventoryDocumentStatus Status { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceBusinessId { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid SellerRef { get; set; }
    public DateTime OccurredAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public DateTime? PostedAt { get; set; }
    public Guid? PostedTransactionRef { get; set; }
    public string? ReasonCode { get; set; }
}
