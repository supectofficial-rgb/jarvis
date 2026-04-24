namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.RejectInventoryDocument;

using OysterFx.AppCore.Shared.Commands;

public class RejectInventoryDocumentCommand : ICommand<Guid>
{
    public Guid DocumentBusinessKey { get; set; }
    public string? ReasonCode { get; set; }
}
