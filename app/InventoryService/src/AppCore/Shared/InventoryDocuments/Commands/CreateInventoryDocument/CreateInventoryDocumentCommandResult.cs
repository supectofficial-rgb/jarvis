namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CreateInventoryDocument;

public class CreateInventoryDocumentCommandResult
{
    public Guid DocumentBusinessKey { get; set; }
    public string DocumentNo { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
