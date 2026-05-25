namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.ChangeInventoryDocumentStatus;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.PostInventoryDocument;
using OysterFx.AppCore.Shared.Commands;

public class ChangeInventoryDocumentStatusCommand : ICommand<bool>
{
    public Guid DocumentBusinessKey { get; set; }
    public string Action { get; set; } = string.Empty;
    public string? Actor { get; set; }
    public string? ReasonCode { get; set; }
    public List<PostInventoryDocumentLineSerialSelectionItem> LineSerials { get; set; } = new();
}
