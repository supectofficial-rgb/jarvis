namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.UpdateInventoryDocumentLine;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using OysterFx.AppCore.Shared.Commands;

public class UpdateInventoryDocumentLineCommand : ICommand<Guid>
{
    public Guid DocumentBusinessKey { get; set; }
    public Guid LineBusinessKey { get; set; }
    public InventoryDocumentCommandLineItem Line { get; set; } = new();
}
