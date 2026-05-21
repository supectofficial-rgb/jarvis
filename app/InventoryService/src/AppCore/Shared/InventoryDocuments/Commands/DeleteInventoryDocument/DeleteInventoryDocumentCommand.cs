namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.DeleteInventoryDocument;

using OysterFx.AppCore.Shared.Commands;

public class DeleteInventoryDocumentCommand : ICommand<Guid>
{
    public Guid DocumentBusinessKey { get; set; }
}
