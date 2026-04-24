namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateIssueDocument;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using OysterFx.AppCore.Shared.Commands;

public class CreateIssueDocumentCommand : ICommand<Guid>
{
    public string? DocumentNo { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceBusinessId { get; set; }
    public Guid WarehouseRef { get; set; }
    public Guid SellerRef { get; set; }
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public string? CorrelationId { get; set; }
    public string? IdempotencyKey { get; set; }
    public string? ReasonCode { get; set; }
    public List<InventoryDocumentCommandLineItem> Lines { get; set; } = new();
}
