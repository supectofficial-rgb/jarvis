namespace Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands.AddInventoryDocumentLine;

using Insurance.InventoryService.AppCore.Shared.InventoryDocuments.Commands;
using OysterFx.AppCore.Shared.Commands;

public class AddInventoryDocumentLineCommand : ICommand<Guid>
{
    public Guid DocumentBusinessKey { get; set; }
    public InventoryDocumentCommandLineItem Line { get; set; } = new();
}
