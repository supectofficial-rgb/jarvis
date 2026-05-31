namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.UpdateInventoryDocument;

using OysterFx.AppCore.Shared.Commands;

public class UpdateInventoryDocumentCommand : ICommand<Guid>
{
    public Guid DocumentBusinessKey { get; set; }
    public string? DocumentNo { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public string? ExternalReferenceNo { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceBusinessId { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid SellerRef { get; set; }
    public string? ReceivedBy { get; set; }
    public string? DeliveredBy { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string? ReasonCode { get; set; }
}
