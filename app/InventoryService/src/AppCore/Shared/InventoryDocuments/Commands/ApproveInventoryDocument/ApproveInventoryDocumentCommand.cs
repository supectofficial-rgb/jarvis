namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.ApproveInventoryDocument;

using OysterFx.AppCore.Shared.Commands;

public class ApproveInventoryDocumentCommand : ICommand<Guid>
{
    public Guid DocumentBusinessKey { get; set; }
    public string ApprovedBy { get; set; } = string.Empty;
}
