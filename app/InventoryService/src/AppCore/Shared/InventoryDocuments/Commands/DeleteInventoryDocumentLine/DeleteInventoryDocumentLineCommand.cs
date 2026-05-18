namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.DeleteInventoryDocumentLine;

using OysterFx.AppCore.Shared.Commands;

public class DeleteInventoryDocumentLineCommand : ICommand<Guid>
{
    public Guid DocumentBusinessKey { get; set; }
    public Guid LineBusinessKey { get; set; }
}
