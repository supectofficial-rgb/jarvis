namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.CancelInventoryDocument;

using OysterFx.AppCore.Shared.Commands;

public class CancelInventoryDocumentCommand : ICommand<Guid>
{
    public Guid DocumentBusinessKey { get; set; }
    public string? ReasonCode { get; set; }
}
