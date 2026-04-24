namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.PostInventoryDocument;

public class PostInventoryDocumentCommandResult
{
    public Guid DocumentBusinessKey { get; set; }
    public Guid TransactionBusinessKey { get; set; }
    public string Status { get; set; } = string.Empty;
}
